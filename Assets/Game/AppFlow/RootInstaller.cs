using System;
using Cysharp.Threading.Tasks;
using Game.AppFlow.AppState;
using Game.AssetManagement;
using Game.AssetManagement.JsConfigs;
using Game.Common.VContainer;
using Game.Configs.UI;
using Game.JsBridge;
using Game.Localization;
using Game.Models.Query;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Game.AppFlow
{
    public class RootInstaller : LifetimeScope
    {
        private const string CONFIG_TAG = "Configs";
        
        [SerializeField] private UiInstallerParameters _uiInstallerParameters = new();

        private AddressablesAssetProvider _configProvider;
        
        private bool _initialized;
        
        public async UniTask InitializeAsync()
        {
            if (_initialized) return;
            _initialized = true;
            _configProvider = new AddressablesAssetProvider();
            await _configProvider.PreloadByTag(CONFIG_TAG);
            Build();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            InstallUI(builder);
            InstallResourceManagement(builder);
            InstallConfigs(builder);
            InstallState(builder);
            InstallStates(builder);
            builder.BindSingleton<PlaceholderLocalizationProvider>();
            builder.BindSingleton<CoreInitializer>();
            builder.BindSingleton<AppStateManager>();
        }

        private void InstallStates(IContainerBuilder builder)
        {
            builder.BindSingleton<AppStateMainMenu>();
            builder.BindSingleton<AppStateGame>();
        }

        private void InstallUI(IContainerBuilder builder)
        {
            UiInstaller.Install(builder, _uiInstallerParameters);
        }
        
        private void InstallResourceManagement(IContainerBuilder builder)
        {
            builder.BindTransient<AddressablesAssetProvider>();
            
            builder.BindSingleton<SpriteProvider>();
        }
        
        private void InstallConfigs(IContainerBuilder builder)
        {
            builder.BindSingleton<AddressablesJsConfigLoader>();
            builder.BindSingleton<JsExecutor>();
            builder.BindSingleton<QueryProvider>();
            
            RegisterConfig<UiWindowsPaths>("Configs/UiWindowsConfig.asset");
            void RegisterConfig<T>(string path) where T : Object 
            {
                var config = _configProvider.GetAsset<T>(path);
                builder.RegisterInstance(config).As<T>();
            }
        }

        private void InstallState(IContainerBuilder builder)
        {
            StateInstaller.Install(builder);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _configProvider?.Dispose();
        }
    }
}