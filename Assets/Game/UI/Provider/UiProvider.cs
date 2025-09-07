using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.AssetManagement;
using Game.UI.Models.Window;
using Game.UI.Presenters.Window;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Game.UI.Provider
{
    public interface IUiProvider
    {
        void OpenSingletonWindow<T>() where T : class, IWindowModel;
        void OpenWindow<T>(T model) where T : class, IWindowModel;
        void CloseWindow();
        UniTask PreloadWindow(Type windowModel);
        UniTask PreloadAllWindows();
        void UnloadAll();
    }

    public static class UiProviderExtensions
    {
        public static async UniTask LoadWindows(this IUiProvider uiProvider, IReadOnlyList<Type> windowModels)
        {
            var loadingTasks = ListPool<UniTask>.Get();

            foreach (var window in windowModels)
            {
                loadingTasks.Add(uiProvider.PreloadWindow(window));
            }

            await UniTask.WhenAll(loadingTasks);

            ListPool<UniTask>.Release(loadingTasks);
        }
    }

    public class UiProvider : IUiProvider, IDisposable
    {
        private readonly IObjectResolver _resolverr;
        private readonly IAssetProvider _assetProvider;
        private readonly IUiScene _uiScene;
        
        private readonly Dictionary<Type, RegistrationEntry> _registrations = new();
        
        private IWindow _currentWindow;
        
        public UiProvider(IObjectResolver resolver, IAssetProvider assetProvider, IUiScene uiScene)
        {
            _resolverr = resolver;
            _assetProvider = assetProvider;
            _uiScene = uiScene;
        }

        public void OpenSingletonWindow<T>() where T : class, IWindowModel
        {
            var model = _resolverr.Resolve<T>();
            OpenWindow(model);
        }

        public void OpenWindow<T>(T model) where T : class, IWindowModel
        {
            var windowType = typeof(T);
            
            CloseWindow();

            var window = _registrations[windowType].WindowFactory.Invoke(model);
            window.OnOpen();

            _currentWindow = window;
            _uiScene.AttachTransform(window.Transform);
        }

        public void CloseWindow()
        {
            if (_currentWindow == null) return;

            _currentWindow.OnClose();
            _currentWindow.Unbind();
            GameObject.Destroy(_currentWindow.Transform.gameObject);

            _currentWindow = null;
        }

        public void RegisterWindow<T>(string prefabPath) where T : class, IWindowModel
        {
            var type = typeof(T);

            if (_registrations.ContainsKey(type))
            {
                Debug.LogError("Already registered window type");
                return;
            }

            IWindow WindowFactory(IWindowModel model)
            {
                var prefab = GetPrefab(prefabPath);
                var instance = GameObject.Instantiate(prefab);

                var window = instance.GetComponent<IWindow>();
                if (window == null)
                {
                    Debug.LogError($"Can't find Window component {type} {prefabPath}");
                    return null;
                }

                window.Bind(model);
                return window;
            }

            var registrationEntry = new RegistrationEntry(type, prefabPath, WindowFactory);

            _registrations[type] = registrationEntry;
        }

        public UniTask PreloadWindow(Type windowModel)
        {
            return _assetProvider.PreloadAsset<GameObject>(_registrations[windowModel].PrefabPath);
        }

        public async UniTask PreloadAllWindows()
        {
            var list = ListPool<UniTask>.Get();
            
            foreach (var registration in _registrations)
            {
                list.Add(PreloadWindow(registration.Key));
            }
            
            await UniTask.WhenAll(list);
            
            ListPool<UniTask>.Release(list);
        }

        public void UnloadAll()
        {
            CloseWindow();
            
            foreach (var registration in _registrations)
            {
                _assetProvider.UnloadAsset(registration.Value.PrefabPath);
            }
        }
        
        private GameObject GetPrefab(string path)
        {
            return _assetProvider.GetAsset<GameObject>(path);
        }
        
        public void Dispose()
        {
            UnloadAll();
        }

        private class RegistrationEntry
        {
            public Type ModelType { get; }
            public string PrefabPath { get; }
            public Func<IWindowModel, IWindow> WindowFactory { get; }

            public RegistrationEntry(Type modelType, string prefabPath, Func<IWindowModel, IWindow> windowFactory)
            {
                ModelType = modelType;
                PrefabPath = prefabPath;
                WindowFactory = windowFactory;
            }
        }
    }
}
