using System;
using System.Collections.Generic;
using Game.AssetManagement;
using Game.Common.VContainer;
using Game.Configs.UI;
using Game.UI.GameLayers;
using Game.UI.GameModels.Windows;
using Game.UI.Models.Window;
using Game.UI.Provider;
using UnityEngine;
using VContainer;

namespace Game.AppFlow
{
    [Serializable]
    public class UiInstallerParameters
    {
        [field:SerializeField] public UiScene MainUiScene { get; private set; }
        [field:SerializeField] public UiScene PopupUiScene { get; private set; }
    }
    
    public static class UiInstaller
    {
        public static void Install(IContainerBuilder builder, UiInstallerParameters parameters)
        {
            builder.Register(resolver =>
            {
                var uiAggregate = new UiAggregate();
                AddUiScene(resolver, uiAggregate, parameters.MainUiScene, RegisterMainWindows, UiLayer.Main);
                AddUiScene(resolver, uiAggregate, parameters.PopupUiScene, RegisterPopupWindows, UiLayer.Popup);
                return uiAggregate;
            }, Lifetime.Singleton)
            .As<IUiAggregate>();

            RegisterSingletonMainWindows(builder);
            RegisterSingletonPopupWindows(builder);
            
            builder.BindSingleton<WindowFactory>();
        }
        
        private static void RegisterMainWindows(UiProviderBuilder builder)
        {
            builder.RegisterWindow<IHomeWindowModel>(config => config.HomeScreenPath);
            builder.RegisterWindow<IBaseGameplayWindowModel>(config => config.BaseGameplayWindowPath);
        }

        private static void RegisterSingletonMainWindows(IContainerBuilder builder)
        {
            builder.BindSingleton<HomeWindowModel>();
            builder.BindSingleton<BaseGameplayWindowModel>();
        }
        
        private static void RegisterPopupWindows(UiProviderBuilder builder)
        {
            
        }
        
        private static void RegisterSingletonPopupWindows(IContainerBuilder builder)
        {
            
        }
        
        private static void AddUiScene(IObjectResolver resolver, UiAggregate uiAggregate, IUiScene uiScene, Action<UiProviderBuilder> registerWindows, UiLayer layer)
        {
            var uiProviderBuilder = new UiProviderBuilder();
            registerWindows(uiProviderBuilder);
            var provider = uiProviderBuilder.CreateProvider(resolver, uiScene);
            
            uiAggregate.SetupLayer(uiScene, provider, layer);
        }
        
        private class UiProviderBuilder
        {
            private readonly List<Action<UiProvider, UiWindowsPaths>> _registrations = new();
            
            public void RegisterWindow<TModel>(Func<UiWindowsPaths, string> pathDelegate)
                where TModel : class, IWindowModel
            {
                _registrations.Add((uiProvider, config) =>
                {
                    uiProvider.RegisterWindow<TModel>(pathDelegate(config));
                });
            }

            public IUiProvider CreateProvider(IObjectResolver resolver, IUiScene uiScene)
            {
                var assetProvider = resolver.Resolve<IAssetProvider>();
                var config = resolver.Resolve<UiWindowsPaths>();

                var provider = new UiProvider(resolver, assetProvider, uiScene);

                foreach (var action in _registrations)
                {
                    action(provider, config);
                }
                
                return provider;
            }
        }
    }
}