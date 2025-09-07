using System;
using System.Collections.Generic;
using System.IO;

namespace Game.TsClassCodeGenerator.Editor
{
    [Serializable]
    public class CodeGenerationReport
    {
        public int FoundInterfaces;
        public int FoundFunctions;

        // Core report lists
        public List<string> NewFiles = new();
        public List<string> ModifiedFiles = new();
        public List<string> DeletedFiles = new();

        // Legacy/compat fields (kept to avoid breaking serialization)
        public List<string> GeneratedInterfaceFiles = new();
        public List<string> UpdatedInterfaceFiles = new();
        public List<string> GeneratedFunctionFiles = new();
        public List<string> UpdatedFunctionFiles = new();
        public List<string> GeneratedFiles = new();
        public List<string> UpdatedFiles = new();
        public List<string> Errors = new();

        public void AddNewFile(string path)
        {
            if (!string.IsNullOrEmpty(path)) NewFiles.Add(path);
        }

        public void AddModifiedFile(string path)
        {
            if (!string.IsNullOrEmpty(path)) ModifiedFiles.Add(path);
        }

        public void AddDeletedFile(string path)
        {
            if (!string.IsNullOrEmpty(path)) DeletedFiles.Add(path);
        }

        public void AddError(string message)
        {
            if (!string.IsNullOrEmpty(message)) Errors.Add(message);
        }
    }
}