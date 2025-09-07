using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.AssetManagement.JsConfigs
{
    public class AddressablesJsConfigLoader : IJsConfigLoader, IDisposable
    {
        private AsyncOperationHandle _handle;
        
        public async UniTask<IReadOnlyList<string>> LoadConfigs(string path)
        {
            if (_handle.IsValid())
            {
                throw new Exception("Loading already started");
            }
            
            var loading = Addressables.LoadAssetsAsync<TextAsset>(path, true);
            _handle = loading;
            
            var result = await loading.ToUniTask();
            
            var resultList = new List<string>();
            foreach (TextAsset jsFile in result)
            {
                resultList.Add(jsFile.text);
            }
            
            return resultList;
        }

        public void Dispose()
        {
            Addressables.Release(_handle);
        }
    }
}