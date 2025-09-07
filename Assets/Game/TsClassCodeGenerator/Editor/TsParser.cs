using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    /// <summary>
    /// Parses TypeScript files and extracts data about interfaces and functions
    /// </summary>
    public class TsParser : ITsParser
    {
        private readonly Dictionary<string, ParsedInterface> _interfaces = new();
        private readonly Dictionary<string, ParsedFunction> _functions = new();
        
        private static readonly HashSet<string> PrimitiveTypes = new()
        {
            "bool", "byte", "int", "long", "ulong",
            "float", "double", "decimal",
            "string"
        };
        
        public IReadOnlyDictionary<string, ParsedInterface> Interfaces => _interfaces;
        public IReadOnlyDictionary<string, ParsedFunction> Functions => _functions;
        
        public void Parse(string tsPath)
        {
            if (!Directory.Exists(tsPath))
            {
                throw new DirectoryNotFoundException($"TypeScript folder not found: {tsPath}");
            }
            
            _interfaces.Clear();
            _functions.Clear();
            
            var tsFiles = Directory.GetFiles(tsPath, "*.ts", SearchOption.AllDirectories);
            
            foreach (var file in tsFiles)
            {
                ParseFile(file);
            }
            
            Debug.Log($"Parsed {_interfaces.Count} interfaces and {_functions.Count} functions from {tsFiles.Length} files");
        }

        public bool IsPrimitive(string typeName)
        {
            return PrimitiveTypes.Contains(typeName);
        }
        
        private void ParseFile(string filePath)
        {
            var content = File.ReadAllText(filePath);

            ParseInterfaces(content);
            ParseFunctions(content);
        }
        
        private void ParseInterfaces(string content)
        {
            var pattern = @"/\*\*[\s\S]*?@cs-export\s+([^\r\n*]+)[\s\S]*?\*/\s*(?:export\s+)?interface\s+(\w+)\s*\{([^}]*)\}";
            var matches = Regex.Matches(content, pattern);
            
            foreach (Match match in matches)
            {
                var namespacePath = match.Groups[1].Value.Trim();
                var interfaceName = match.Groups[2].Value;
                
                var body = match.Groups[3].Value;
                
                var parsedInterface = new ParsedInterface(
                    ConvertTsTypeNameToCsName(interfaceName),
                    namespacePath,
                    ParseProperties(body)
                );

                _interfaces[parsedInterface.CsName] = parsedInterface;
            }
        }
        
        private List<ParsedProperty> ParseProperties(string body)
        {
            var properties = new List<ParsedProperty>();
            var lines = body.Split('\n');
            
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || 
                    trimmed.StartsWith("//") || 
                    trimmed.StartsWith("/*") || 
                    trimmed.StartsWith("*/") || 
                    trimmed.StartsWith("*"))
                    continue;
                
                var match = Regex.Match(trimmed, @"(\w+)\s*(\?)?\s*:\s*([^;,]+)");
                if (match.Success)
                {
                    properties.Add(new ParsedProperty(
                        match.Groups[1].Value,
                        ConvertTsTypeNameToCsName(match.Groups[3].Value.Trim()),
                        match.Groups[2].Success));
                }
            }
            
            return properties;
        }
        
        private void ParseFunctions(string content)
        {
            var docPattern = @"/\*\*[\s\S]*?@cs-export\s+([^\r\n*]+)[\s\S]*?\*/";
            var docMatches = Regex.Matches(content, docPattern);
            
            foreach (Match docMatch in docMatches)
            {
                var csExport = docMatch.Groups[1].Value.Trim();

                var afterIndex = docMatch.Index + docMatch.Length;
                if (afterIndex >= content.Length)
                    continue;

                var after = content.Substring(afterIndex);
                var funcMatch = Regex.Match(after, @"^\s*(?:export\s+)?(?:async\s+)?function\s+(\w+)\s*\(([^)]*)\)\s*:\s*([^\{]+)");
                if (!funcMatch.Success)
                    continue;

                var functionName = funcMatch.Groups[1].Value;
                var parameters = funcMatch.Groups[2].Value;
                var returnTypeComplete = funcMatch.Groups[3].Value.Trim();

                var parts = csExport.Split(new[] { ':' }, 2, StringSplitOptions.None);
                var namespacePath = parts[0].Trim();
                var outerClass = (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1]))
                    ? parts[1].Trim()
                    : "TsQueries";

                var returnOptional = Regex.IsMatch(returnTypeComplete, "\\|\\s*undefined");
                var returnType = returnOptional
                    ? Regex.Split(returnTypeComplete, "\\|\\s*undefined")[0].Trim()
                    : returnTypeComplete;

                var parsedFunction = new ParsedFunction(
                    ConvertFuncNameToCsName(functionName),
                    functionName,
                    namespacePath,
                    outerClass,
                    ConvertTsTypeNameToCsName(returnType),
                    returnOptional,
                    ParseParameters(parameters)
                );

                _functions[parsedFunction.CsName] = parsedFunction;
            }
        }
        
        private List<ParsedParameter> ParseParameters(string parametersStr)
        {
            var parameters = new List<ParsedParameter>();
            
            if (string.IsNullOrWhiteSpace(parametersStr))
                return parameters;
            
            var parts = parametersStr.Split(',');
            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;
                
                var match = Regex.Match(trimmed, @"(\w+)\s*(\?)?\s*:\s*([^=]+)(?:\s*=\s*[^,]+)?");
                if (match.Success)
                {
                    var name = match.Groups[1].Value;
                    var hasQuestionOptional = match.Groups[2].Success;
                    var typeRaw = match.Groups[3].Value.Trim();
                    var hasUnionUndefined = Regex.IsMatch(typeRaw, "\\|\\s*undefined");
                    if (hasUnionUndefined)
                    {
                        typeRaw = Regex.Split(typeRaw, "\\|\\s*undefined")[0].Trim();
                    }
                    parameters.Add(new ParsedParameter(
                        name,
                        ConvertTsTypeNameToCsName(typeRaw),
                        hasQuestionOptional || hasUnionUndefined));
                }
            }
            
            return parameters;
        }
        
        private string ConvertTsTypeNameToCsName(string tsName)
        {
            if (string.IsNullOrWhiteSpace(tsName)) return tsName;

            // Map known TS primitives and aliases to C#
            var trimmed = tsName.Trim();
            switch (trimmed)
            {
                case "boolean":
                    return "bool";
                case "number":
                    return "int";
                case "string":
                case "int":
                case "float":
                case "double":
                    return trimmed;
            }

            // Handle arrays (e.g. Type[])
            if (trimmed.EndsWith("[]"))
            {
                var element = ConvertTsTypeNameToCsName(trimmed.Substring(0, trimmed.Length - 2));
                return element + "[]";
            }

            // Drop leading I for interface-like names (IThing)
            if (trimmed.StartsWith("I") && trimmed.Length > 1 && char.IsUpper(trimmed[1]))
            {
                trimmed = trimmed.Substring(1);
            }

            // PascalCase
            if (char.IsLower(trimmed[0]))
            {
                return char.ToUpper(trimmed[0]) + trimmed.Substring(1);
            }

            return trimmed;
        }
        
        private string ConvertFuncNameToCsName(string tsName)
        {
            if (char.IsLower(tsName[0]))
            {
                return char.ToUpper(tsName[0]) + tsName.Substring(1);
            }
            
            return tsName;
        }
    }
    
    public class ParsedInterface
    {
        public readonly string CsName;
        public readonly string Namespace;
        public readonly List<ParsedProperty> Properties;

        public ParsedInterface(string csName, string @namespace, List<ParsedProperty> properties)
        {
            CsName = csName;
            Namespace = @namespace;
            Properties = properties ?? new List<ParsedProperty>();
        }
    }
    
    public class ParsedProperty
    {
        public readonly string Name;
        public readonly string Type;
        public readonly bool IsOptional;

        public ParsedProperty(string name, string type, bool isOptional)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
        }
    }
    
    public class ParsedFunction
    {
        public readonly string CsName;
        public readonly string TsName;
        public readonly string Namespace;
        public readonly string OuterPartialClassName;
        public readonly string ReturnType;
        public readonly bool ReturnOptional;
        public readonly List<ParsedParameter> Parameters;

        public ParsedFunction(string csName, string tsName, string @namespace, string outerPartialClassName, string returnType, bool returnOptional, List<ParsedParameter> parameters)
        {
            CsName = csName;
            TsName = tsName;
            Namespace = @namespace;
            OuterPartialClassName = outerPartialClassName;
            ReturnType = returnType;
            ReturnOptional = returnOptional;
            Parameters = parameters ?? new List<ParsedParameter>();
        }
    }
    
    public class ParsedParameter
    {
        public readonly string Name;
        public readonly string Type;
        public readonly bool IsOptional;

        public ParsedParameter(string name, string type, bool isOptional)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
        }
    }
}
