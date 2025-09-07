using Cysharp.Threading.Tasks;
using Game.AssetManagement.JsConfigs;
using Game.JsBridge;
using Game.SaveSystem;
using UnityEngine;

namespace Game.AppFlow
{
    public interface ICoreInitializer
    {
        UniTask InitializeIfNeededAsync();
    }
    
    public class CoreInitializer : ICoreInitializer
    {
        private readonly ISaveDataManager _saveDataManager;
        private readonly IJsExecutor _jsExecutor;
        private readonly IJsConfigLoader _configLoader;
        
        public bool _initalized;

        public CoreInitializer(ISaveDataManager saveDataManager)
        {
            _saveDataManager = saveDataManager;
        }

        public async UniTask InitializeIfNeededAsync()
        {
            if (_initalized) return;
            _initalized = true;
            
            Application.targetFrameRate = 60;
            _saveDataManager.Initialize();
            await _saveDataManager.LoadAllDataAsync();
        }
    }
}