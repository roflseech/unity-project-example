using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UniRx;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.AssetManagement
{
    public class AddressablesAssetProvider : IAssetProvider, IDisposable
    {
        private readonly Dictionary<string, PrefabLifetimeEntry> _assets = new();
        private readonly Subject<string> _assetLoadedSubject = new();
        
        public async UniTask PreloadAsset<T>(string path) where T : Object
        {
            if (_assets.ContainsKey(path))
            {
                Debug.LogError($"Prefab already preloaded: {path}");
                return;
            }
            
            var entry = new PrefabLifetimeEntry();
            _assets.Add(path, entry);
            try
            {
                var handle = Addressables.LoadAssetAsync<T>(path);
                entry.Handle = handle;
            
                entry.Prefab = await handle;
            }
            catch (Exception e)
            {
                Debug.LogError($"Cannot load asset with key {path}, exception: {e}");
                entry.Handle = default;
                entry.Prefab = null;
                return;
            }

            _assetLoadedSubject.OnNext(path);
        }

        public T GetAsset<T>(string path) where T : Object
        {
            return _assets[path].Prefab as T;
        }

        public void UnloadAsset(string path)
        {
            if (_assets.TryGetValue(path, out var entry))
            {
                if (entry.Handle.IsValid())
                {
                    Addressables.Release(entry.Handle);
                }
                _assets.Remove(path);
            }
            else
            {
                Debug.LogError($"Failed to unload asset with key {path}");
            }
        }

        public bool TryGetAsset<T>(string path, out Object asset) where T : Object
        {
            if (_assets.TryGetValue(path, out var entry) && entry.Prefab != null)
            {
                asset = entry.Prefab as T;
                return asset != null;
            }
            
            asset = null;
            return false;
        }

        public AssetState GetAssetState(string path)
        {
            if (!_assets.TryGetValue(path, out var entry))
            {
                return AssetState.NotLoaded;
            }

            if (entry.Prefab == null && entry.Handle.IsValid()) return AssetState.Loading;
            
            return AssetState.Loaded;
        }

        public IObservable<string> AssetLoaded => _assetLoadedSubject.AsObservable();
        
        public async UniTask PreloadByTag(string tag)
        {
            var locationsHandle = Addressables.LoadResourceLocationsAsync(tag);
            var locations = await locationsHandle;
            
            var tasks = ListPool<UniTask>.Get();
            
            foreach (var location in locations)
            {
                tasks.Add(PreloadAsset<Object>(location.PrimaryKey));
            }
            
            await UniTask.WhenAll(tasks);
            
            ListPool<UniTask>.Release(tasks);
        }

        private class PrefabLifetimeEntry
        {
            public AsyncOperationHandle Handle;
            public Object Prefab;
        }

        public void Dispose()
        {
            foreach (var entry in _assets.Values)
            {
                if (entry.Handle.IsValid())
                {
                    Addressables.Release(entry.Handle);
                }
            }
            _assets.Clear();
            _assetLoadedSubject?.Dispose();
        }
    }
}