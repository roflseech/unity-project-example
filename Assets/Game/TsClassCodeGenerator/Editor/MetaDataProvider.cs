using UnityEngine;

namespace Game.TsClassCodeGenerator.Editor
{
    public class MetaDataProvider : IMetaDataProvider
    {
        public string GetCodeBaseRoot()
        {
            return Application.dataPath;
        }
    }
}