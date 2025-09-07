using System;
using System.Collections.Generic;

namespace Game.TsClassCodeGenerator.Editor
{
    [Serializable]
    public class TsClassGeneratorReport
    {
        public int TsFilesScanned;
        public int InterfacesFound;
        public List<string> FilesCreated = new();
        public List<string> FilesModified = new();
        public List<string> FilesDeleted = new();
        public List<FileChange> FileChanges = new();
    }

    [Serializable]
    public class FileChange
    {
        public string FileName;
        public List<string> FieldsAdded = new();
        public List<string> FieldsRemoved = new();
    }

    public class InterfaceData
    {
        public string TSName { get; set; }
        public string CSNamespace { get; set; }
        public string CSName { get; set; }
        public string CSFolder { get; set; }
        public List<Property> Properties { get; set; } = new();
    }

    public class Property
    {
        public string Name { get; set; }
        public string TSType { get; set; }
        public string CSType { get; set; }
        public bool IsOptional { get; set; }
    }
}