using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    /// <summary>
    /// Utility methods for TypeScript code generation.
    /// </summary>
    public static class TsCodeGeneratorUtils
    {
        /// <summary>
        /// Converts TypeScript types to C# types.
        /// </summary>
        public static string ConvertType(string type) => type.Trim() switch
        {
            "string" => "string",
            "number" => "float",
            "int" => "int",
            "float" => "float",
            "double" => "double",
            "boolean" => "bool",
            _ => type.Trim() // For custom types, keep original type (conversion happens in GetFieldType)
        };

        /// <summary>
        /// Converts camelCase to PascalCase.
        /// </summary>
        public static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            // Handle camelCase to PascalCase conversion
            if (char.IsLower(str[0]))
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str;
        }

        /// <summary>
        /// Converts PascalCase to camelCase.
        /// </summary>
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Checks if a type is a primitive C# type.
        /// </summary>
        public static bool IsPrimitive(string type) => type is "string" or "float" or "int" or "double" or "bool" or "boolean" or "number";

        /// <summary>
        /// Gets the primitive converter method name for Jint.
        /// </summary>
        public static string GetPrimitiveConverter(string type) => type switch
        {
            "string" => "String",
            "float" => "Float",
            "int" => "Int", 
            "double" => "Double",
            "bool" => "Boolean",
            _ => type
        };

        /// <summary>
        /// Checks if a type is a reference type (string or array).
        /// </summary>
        public static bool IsRef(string type) => type == "string" || type.Contains("[]");

        /// <summary>
        /// Converts a JsValue to the appropriate C# type.
        /// </summary>
        public static string ConvertJsValue(string type, string expr) => type switch
        {
            "string" => $"{expr}.AsString()",
            "float" => $"{expr}.AsFloat()",
            "int" => $"{expr}.AsInt()",
            "double" => $"{expr}.AsDouble()",
            "bool" => $"{expr}.AsBoolean()",
            _ => $"{expr}.As{type}()"
        };

        /// <summary>
        /// Converts TypeScript interface/type name to C# class name.
        /// Removes only the first 'I' prefix if present to match generated C# class names.
        /// </summary>
        public static string ConvertTsTypeToCsType(string tsType)
        {
            // Remove only the first 'I' prefix if present (TypeScript interface to C# class)
            if (tsType.StartsWith("I") && tsType.Length > 1)
            {
                // Only remove the first 'I', keep the rest
                return tsType.Substring(1);
            }
            return tsType;
        }

        /// <summary>
        /// Centralized method to convert TypeScript type to C# type.
        /// Handles both primitives and custom types with proper 'I' prefix removal.
        /// </summary>
        public static string ConvertTsToCsType(string tsType)
        {
            var trimmed = tsType.Trim();
            
            // Handle primitives first
            var primitive = trimmed switch
            {
                "string" => "string",
                "number" => "float",
                "int" => "int",
                "float" => "float",
                "double" => "double",
                "boolean" => "bool",
                _ => null
            };
            
            if (primitive != null)
                return primitive;
            
            // Handle custom types - remove 'I' prefix if present
            return ConvertTsTypeToCsType(trimmed);
        }

        /// <summary>
        /// Gets the field type for a property.
        /// </summary>
        public static string GetFieldType(string type) => type switch
        {
            "string" => "string",
            "float" => "float",
            "int" => "int",
            "double" => "double",
            "bool" => "bool",
            var t when t.EndsWith("[]") => ConvertType(t.Substring(0, t.Length - 2)) + "[]",
            _ => type
        };
    }

    /// <summary>
    /// Utility methods for TypeScript file parsing.
    /// </summary>
    public static class TsFileParser
    {
        /// <summary>
        /// Gets all exported types from TypeScript files.
        /// </summary>
        public static HashSet<string> GetExportedTypes(string tsPath)
        {
            var exportedTypes = new HashSet<string>();
            
            if (!Directory.Exists(tsPath))
                return exportedTypes;

            var tsFiles = Directory.GetFiles(tsPath, "*.ts", SearchOption.AllDirectories);
            foreach (var file in tsFiles)
            {
                var content = File.ReadAllText(file);
                var pattern = @"/\*\*[\s\S]*?@cs-export\s+([^\r\n*]+)[\s\S]*?\*/\s*(?:export\s+)?interface\s+(\w+)\s*\{";
                
                foreach (Match m in Regex.Matches(content, pattern))
                {
                    var interfaceName = m.Groups[2].Value;
                    // Convert interface name to class name using centralized method
                    var className = TsCodeGeneratorUtils.ConvertTsToCsType(interfaceName);
                    exportedTypes.Add(className);
                }
            }
            
            return exportedTypes;
        }

        /// <summary>
        /// Gets the namespace for a type by scanning exported interfaces.
        /// </summary>
        public static string GetNamespaceForType(string type, string tsPath)
        {
            if (!Directory.Exists(tsPath))
                return null;

            var tsFiles = Directory.GetFiles(tsPath, "*.ts", SearchOption.AllDirectories);
            foreach (var file in tsFiles)
            {
                var content = File.ReadAllText(file);
                var pattern = @"/\*\*[\s\S]*?@cs-export\s+([^\r\n*]+)[\s\S]*?\*/\s*(?:export\s+)?interface\s+(\w+)\s*\{";
                
                foreach (Match m in Regex.Matches(content, pattern))
                {
                    var namespacePart = m.Groups[1].Value.Trim();
                    var interfaceName = m.Groups[2].Value;
                    var className = TsCodeGeneratorUtils.ConvertTsToCsType(interfaceName);
                    
                    if (className == type)
                    {
                        return namespacePart;
                    }
                }
            }
            
            return null;
        }
    }
} 