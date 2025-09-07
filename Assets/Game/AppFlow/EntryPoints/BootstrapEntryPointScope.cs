using System.Threading;
using Cysharp.Threading.Tasks;
using Game.AppFlow.AppState;
using Game.AssetManagement;
using Game.SaveSystem;
using Game.UI.GameLayers;
using Game.UI.GameModels.Windows;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.AppFlow.EntryPoints
{
    public class BootstrapEntryPointScope : BaseEntryPointScope
    {
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterEntryPoint<EntryPoint>();
        }

        private class EntryPoint : BaseEntryPoint
        {
            private readonly IAppStateManager _appStateManager;
            
            public EntryPoint(ICoreInitializer coreInitializer, IAppStateManager appStateManager) : base(coreInitializer)
            {
                _appStateManager = appStateManager;
            }

            protected override UniTask InitializeAsync()
            {
                _appStateManager.GoToState<AppStateMainMenu>();
                return UniTask.CompletedTask;
            }
        }
    }
}