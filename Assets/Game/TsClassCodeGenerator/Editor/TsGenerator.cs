using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    /// <summary>
    /// Generates C# code from parsed TypeScript data (TsParser)
    /// and performs cleanup of obsolete generated files.
    /// </summary>
    public class TsGenerator : ITsCodeGenerator
    {
        private ITsParser _parser;
        private IMetaDataProvider _metaDataProvider;

        private CodeGenerationReport _report;
        
        public CodeGenerationReport GenerateCode(ITsParser parser, IMetaDataProvider metaDataProvider)
        {
            _parser = parser;
            _metaDataProvider = metaDataProvider;
            _report = new();
            _report.FoundInterfaces = _parser.Interfaces.Count;
            _report.FoundFunctions = _parser.Functions.Count;

            var plannedFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            BuildPlannedInterfaceFiles(plannedFiles);
            BuildPlannedFunctionFiles(plannedFiles);
            ApplyPlannedChanges(plannedFiles);

            return _report;
        }

        private void BuildPlannedInterfaceFiles(Dictionary<string, string> plannedFiles)
        {
            foreach (var parsed in _parser.Interfaces.Values)
            {
                try
                {
                    var filePath = GetOutputPath(parsed);
                    var content = GenerateInterfaceCode(parsed);
                    plannedFiles[filePath] = content;
                }
                catch (Exception e)
                {
                    _report.AddError($"Failed to prepare {parsed.CsName}: {e.Message}");
                }
            }
        }

        private bool IsGeneratedFile(string file)
        {
            var content = File.ReadAllText(file);
            if (file.IndexOf("TsGenerator", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }
            return content.Contains("[TsGenerated]") || content.Contains("[TsGeneratedFunc]");
        }
        
        private void ApplyPlannedChanges(Dictionary<string, string> plannedFiles)
        {
            var rootFolder = Path.Combine(_metaDataProvider.GetCodeBaseRoot(), "Game", "Generated");
            if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);

            // Compare and write new/modified
            foreach (var kv in plannedFiles)
            {
                var filePath = kv.Key;
                var newContent = kv.Value;

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, newContent);
                    _report.AddNewFile(ToProjectRelative(filePath));
                }
                else
                {
                    var oldContent = File.ReadAllText(filePath);
                    if (!string.Equals(oldContent, newContent, StringComparison.Ordinal))
                    {
                        File.WriteAllText(filePath, newContent);
                        _report.AddModifiedFile(ToProjectRelative(filePath));
                    }
                }
            }

            // Detect and delete obsolete generated files
            if (Directory.Exists(rootFolder))
            {
                var allCs = Directory.GetFiles(rootFolder, "*.cs", SearchOption.AllDirectories);
                foreach (var file in allCs)
                {
                    if (!IsGeneratedFile(file)) continue;
                    if (plannedFiles.ContainsKey(file)) continue;
                    try
                    {
                        File.Delete(file);
                        _report.AddDeletedFile(ToProjectRelative(file));
                    }
                    catch (Exception e)
                    {
                        _report.AddError($"Failed to delete obsolete generated file '{file}': {e.Message}");
                    }
                }
            }
        }
        
        private string GenerateInterfaceCode(ParsedInterface parsed)
        {
            var sb = new StringBuilder();
            
            var usings = new HashSet<string>(StringComparer.Ordinal)
            {
                "System",
                "System.Linq",
                "Game.TsClassCodeGenerator.Runtime",
                "Jint.Native",
                "Jint",
                "Game.JsBridge"
            };

            foreach (var prop in parsed.Properties)
            {
                var baseType = prop.Type.Replace("[]", string.Empty);
                if (_parser.IsPrimitive(baseType))
                {
                    continue;
                }
                if (_parser.Interfaces.TryGetValue(baseType, out var parsedDep))
                {
                    usings.Add(parsedDep.Namespace);
                }
            }

            //usings
            foreach (var u in usings)
            {
                sb.AppendLine($"using {u};");
            }
            sb.AppendLine();
            
            // namespace
            sb.AppendLine($"namespace {parsed.Namespace}");
            sb.AppendLine("{");

            // struct
            sb.AppendLine("    [TsGenerated]");
            sb.AppendLine($"    public readonly struct {parsed.CsName}");
            sb.AppendLine("    {");
            
            //properties
            foreach (var prop in parsed.Properties)
            {
                var propName = ToPascalCase(prop.Name);
                var fieldType = GetFieldTypeForProperty(prop);
                sb.AppendLine($"        public readonly {fieldType} {propName};");
            }
            
            //constructor
            if (parsed.Properties.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"        public {parsed.CsName}(");
                sb.AppendLine("            " + string.Join(",\n            ", parsed.Properties.Select(p => $"{GetParameterTypeForProperty(p)} {ToCamelCase(p.Name)}")));
                sb.AppendLine("        )");
                sb.AppendLine("        {");
                foreach (var prop in parsed.Properties)
                {
                    sb.AppendLine($"            {ToPascalCase(prop.Name)} = {ToCamelCase(prop.Name)};");
                }
                sb.AppendLine("        }");
            }
            sb.AppendLine("    }");
            sb.AppendLine();

            // Converter extension
            sb.AppendLine($"    public static class {parsed.CsName}Converter");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static {parsed.CsName} As{parsed.CsName}(this JsValue jsValue)");
            sb.AppendLine("        {");
            foreach (var prop in parsed.Properties)
            {
                var tmpVar = $"tmp{ToPascalCase(prop.Name)}";
                sb.AppendLine($"            var {tmpVar} = jsValue.Get(\"{prop.Name}\");");

                var csType = prop.Type;
                var getterBody = GetJsValueGetterBody(csType, tmpVar);
                if (prop.IsOptional)
                {
                    if (IsArrayType(csType))
                    {
                        var elemType = GetArrayElementType(csType);
                        sb.AppendLine($"            var {ToCamelCase(prop.Name)} = {tmpVar}.IsUndefined() ? System.Array.Empty<{elemType}>() : {getterBody};");
                    }
                    else if (IsValueType(csType))
                    {
                        sb.AppendLine($"            var {ToCamelCase(prop.Name)} = {tmpVar}.IsUndefined() ? ({csType}?)null : {getterBody};");
                    }
                    else if (IsStringType(csType))
                    {
                        sb.AppendLine($"            var {ToCamelCase(prop.Name)} = {tmpVar}.IsUndefined() ? null : {getterBody};");
                    }
                    else
                    {
                        // Fallback for other reference-like types
                        sb.AppendLine($"            var {ToCamelCase(prop.Name)} = {tmpVar}.IsUndefined() ? null : {getterBody};");
                    }
                }
                else
                {
                    sb.AppendLine($"            var {ToCamelCase(prop.Name)} = {getterBody};");
                }
            }
            if (parsed.Properties.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine($"            return new {parsed.CsName}({string.Join(", ", parsed.Properties.Select(p => ToCamelCase(p.Name)))});");
            }
            else
            {
                sb.AppendLine($"            return new {parsed.CsName}();");
            }
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GetOutputPath(ParsedInterface parsedInterface)
        {
            var root = Path.Combine(_metaDataProvider.GetCodeBaseRoot(), "Game", "Generated");
            var nsRelative = parsedInterface.Namespace
                .Replace("Game.Generated.", string.Empty)
                .Replace('.', Path.DirectorySeparatorChar);
            var dir = Path.Combine(root, nsRelative);
            var filename = parsedInterface.CsName + ".cs";
            return Path.Combine(dir, filename);
        }
        private string GetOutputPath(ParsedFunction parsedFunction)
        {
            var root = Path.Combine(_metaDataProvider.GetCodeBaseRoot(), "Game", "Generated");
            var nsRelative = parsedFunction.Namespace
                .Replace("Game.Generated.", string.Empty)
                .Replace('.', Path.DirectorySeparatorChar);
            var dir = Path.Combine(root, nsRelative);
            var filename = parsedFunction.CsName + ".cs";
            return Path.Combine(dir, filename);
        }

        private void BuildPlannedFunctionFiles(Dictionary<string, string> plannedFiles)
        {
            foreach (var func in _parser.Functions.Values)
            {
                try
                {
                    var filePath = GetOutputPath(func);
                    var content = GenerateFunctionCode(func);
                    plannedFiles[filePath] = content;
                }
                catch (Exception e)
                {
                    _report.AddError($"Failed to prepare function {func.CsName}: {e.Message}");
                }
            }
        }

        private string GenerateFunctionCode(ParsedFunction func)
        {
            var sb = new StringBuilder();

            // Collect usings
            var usings = new HashSet<string>(StringComparer.Ordinal)
            {
                "System",
                "System.Linq",
                "Game.JsBridge",
                "Game.TsClassCodeGenerator.Runtime",
                "Jint.Native",
                "Jint",
            };

            var requiredTypes = new HashSet<string>();
            requiredTypes.Add(func.ReturnType.Replace("[]", string.Empty));

            foreach (var p in func.Parameters)
            {
                requiredTypes.Add(p.Type.Replace("[]", string.Empty));
            }

            foreach (var requiredType in requiredTypes)
            {
                if (_parser.IsPrimitive(requiredType) || requiredType == "void")
                {
                    continue;
                }

                if (_parser.Interfaces.TryGetValue(requiredType, out var parsedDep))
                {
                    usings.Add(parsedDep.Namespace);
                }
            }

            foreach (var u in usings)
            {
                sb.AppendLine($"using {u};");
            }
            sb.AppendLine();
            
            sb.AppendLine($"namespace {func.Namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public partial class {func.OuterPartialClassName}");
            sb.AppendLine("    {");

            sb.AppendLine("        [TsGeneratedFunc]");
            sb.AppendLine($"        public class {func.CsName} : BaseQuery");
            sb.AppendLine("        {");
            sb.AppendLine();

            var returnTypeComplete = GetFunctionReturnType(func);
            
            // Execute signature
            sb.Append($"            public {returnTypeComplete} Execute(");
            if (func.Parameters.Any())
            {
                sb.Append(string.Join(", ", func.Parameters.Select(p => $"{GetParameterTypeForFunctionParam(p)} {ToCamelCase(p.Name)}")));
            }
            sb.AppendLine(")");
            sb.AppendLine("            {");

            var paramArray = func.Parameters.Any()
                ? string.Join(", ", func.Parameters.Select(p => ToCamelCase(p.Name)))
                : string.Empty;

            if (func.ReturnType == "void")
            {
                sb.AppendLine($"                ExecuteJsFunction<object>(\"{func.TsName}\", new object[] {{ {paramArray} }}, x => default(object));");
            }
            else if (func.ReturnOptional)
            {
                var convBody = GetJsValueConverterBody(func.ReturnType, "x");
                if (IsArrayType(func.ReturnType))
                {
                    var elemType = GetArrayElementType(func.ReturnType);
                    sb.AppendLine($"                return ExecuteJsFunction<{returnTypeComplete}>(\"{func.TsName}\", new object[] {{ {paramArray} }}, x => x.IsUndefined() ? System.Array.Empty<{elemType}>() : {convBody});");
                }
                else if (IsValueType(func.ReturnType))
                {
                    sb.AppendLine($"                return ExecuteJsFunction<{returnTypeComplete}>(\"{func.TsName}\", new object[] {{ {paramArray} }}, x => x.IsUndefined() ? default : {convBody});");
                }
                else if (IsStringType(func.ReturnType))
                {
                    sb.AppendLine($"                return ExecuteJsFunction<{returnTypeComplete}>(\"{func.TsName}\", new object[] {{ {paramArray} }}, x => x.IsUndefined() ? null : {convBody});");
                }
                else
                {
                    sb.AppendLine($"                return ExecuteJsFunction<{returnTypeComplete}>(\"{func.TsName}\", new object[] {{ {paramArray} }}, x => x.IsUndefined() ? null : {convBody});");
                }
            }
            else
            {
                var converter = GetJsValueConverter(func.ReturnType);
                sb.AppendLine($"                return ExecuteJsFunction<{returnTypeComplete}>(\"{func.TsName}\", new object[] {{ {paramArray} }}, {converter});");
            }

            sb.AppendLine("            }");
            sb.AppendLine("        }");

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        
        private string ToProjectRelative(string fullPath)
        {
            var root = _metaDataProvider.GetCodeBaseRoot();
            if (fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                var rel = fullPath.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                rel = rel.Replace(Path.DirectorySeparatorChar, '/');
                return string.IsNullOrEmpty(rel) ? "Assets" : $"Assets/{rel}";
            }
            return fullPath.Replace(Path.DirectorySeparatorChar, '/');
        }
        

        private string GetJsValueGetter(string csType, string jsValueExpr)
        {
            return GetJsValueGetterBody(csType, jsValueExpr);
        }

        private string GetJsValueConverter(string csType)
        {
            return $"x => {GetJsValueConverterBody(csType, "x")}";
        }

        private string GetJsValueGetterBody(string csType, string jsValueExpr)
        {
            return csType switch
            {
                "string" => $"Game.JsBridge.BaseJsConverters.AsString({jsValueExpr})",
                "float" => $"Game.JsBridge.BaseJsConverters.AsFloat({jsValueExpr})",
                "int" => $"Game.JsBridge.BaseJsConverters.AsInt({jsValueExpr})",
                "double" => $"Game.JsBridge.BaseJsConverters.AsDouble({jsValueExpr})",
                "bool" => $"Game.JsBridge.BaseJsConverters.AsBoolean({jsValueExpr})",
                "object" => $"{jsValueExpr}",
                _ when IsArrayType(csType) => $"{jsValueExpr}.AsArray().Select(v => v.As{GetArrayElementType(csType)}()).ToArray()",
                _ => $"{jsValueExpr}.As{csType}()"
            };
        }

        private string GetJsValueConverterBody(string csType, string jsValueVar)
        {
            return csType switch
            {
                "string" => $"Game.JsBridge.BaseJsConverters.AsString({jsValueVar})",
                "float" => $"Game.JsBridge.BaseJsConverters.AsFloat({jsValueVar})",
                "int" => $"Game.JsBridge.BaseJsConverters.AsInt({jsValueVar})",
                "double" => $"Game.JsBridge.BaseJsConverters.AsDouble({jsValueVar})",
                "bool" => $"Game.JsBridge.BaseJsConverters.AsBoolean({jsValueVar})",
                "object" => $"{jsValueVar}",
                _ when IsArrayType(csType) => $"{jsValueVar}.AsArray().Select(v => v.As{GetArrayElementType(csType)}()).ToArray()",
                _ => $"{jsValueVar}.As{csType}()"
            };
        }

        private bool IsArrayType(string csType) => csType.EndsWith("[]");
        private string GetArrayElementType(string csType) => csType[0].ToString().ToUpper() + 
                                                             csType.Substring(1, csType.Length - 3);
        private bool IsStringType(string csType) => csType == "string";
        private bool IsPrimitiveValueType(string csType)
        {
            switch (csType)
            {
                case "bool":
                case "byte":
                case "int":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "decimal":
                    return true;
                default:
                    return false;
            }
        }
        private bool IsValueType(string csType)
        {
            if (IsArrayType(csType) || IsStringType(csType)) return false;
            if (IsPrimitiveValueType(csType)) return true;
            var baseType = csType.Replace("[]", string.Empty);
            return _parser.Interfaces.ContainsKey(baseType);
        }

        private string GetFunctionReturnType(ParsedFunction func)
        {
            var baseType = func.ReturnType;
            if (baseType == "void") return "void";
            if (func.ReturnOptional && IsValueType(baseType))
            {
                return baseType + "?";
            }
            return baseType;
        }

        private string GetParameterTypeForFunctionParam(ParsedParameter param)
        {
            var baseType = param.Type;
            if (param.IsOptional && IsValueType(baseType))
            {
                return baseType + "?";
            }
            return baseType;
        }

        private string GetFieldTypeForProperty(ParsedProperty prop)
        {
            var baseType = prop.Type;
            if (prop.IsOptional && IsValueType(baseType))
            {
                return baseType + "?";
            }
            return baseType;
        }

        private string GetParameterTypeForProperty(ParsedProperty prop)
        {
            var baseType = prop.Type;
            if (prop.IsOptional && IsValueType(baseType))
            {
                return baseType + "?";
            }
            return baseType;
        }

        private static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}

