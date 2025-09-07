using System;
using System.Collections.Generic;

namespace Game.TsClassCodeGenerator.Editor
{
    [Serializable]
    public class TsFuncGeneratorReport
    {
        public int TsFilesScanned;
        public int FunctionsFound;
        public List<string> FilesCreated = new();
        public List<string> FilesModified = new();
        public List<string> FilesDeleted = new();
        public List<FunctionChange> FunctionChanges = new();
        public List<TypeValidationError> TypeValidationErrors = new();
    }

    [Serializable]
    public class FunctionChange
    {
        public string FileName;
        public List<string> ParametersAdded = new();
        public List<string> ParametersRemoved = new();
        public string ReturnTypeChanged;
    }

    [Serializable]
    public class TypeValidationError
    {
        public string FunctionName;
        public string FileName;
        public string TypeName;
        public string ErrorReason;
        public List<string> AvailableTypes = new();
    }
} 