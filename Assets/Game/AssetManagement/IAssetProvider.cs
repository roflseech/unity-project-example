using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using UniRx;
using Object = UnityEngine.Object;

namespace Game.AssetManagement
{
    public enum AssetState { NotLoaded, Loaded, Loading }
    
    public interface IAssetProvider
    {
        UniTask PreloadAsset<T>(string path) where T : Object;
        T GetAsset<T>(string path) where T : Object;
        void UnloadAsset(string path);
        bool TryGetAsset<T>(string path, out Object asset) where T : Object;
        AssetState GetAssetState(string path);
        IObservable<string> AssetLoaded { get; }
        UniTask PreloadByTag(string tag);
    }

    public static class IAssetProviderExtensions
    {
        public static IObservable<T> LoadAssetAsObservable<T>(this IAssetProvider assetProvider, string path) where T : Object
        {
            var state = assetProvider.GetAssetState(path);

            if (state == AssetState.Loaded)
            {
                return Observable.Return(assetProvider.GetAsset<T>(path));
            }

            if (state == AssetState.NotLoaded)
            {
                assetProvider.PreloadAsset<T>(path).Forget();
            }
            
            state = assetProvider.GetAssetState(path);
            if (state == AssetState.Loaded)
            {
                return Observable.Return(assetProvider.GetAsset<T>(path));
            }

            return assetProvider.AssetLoaded
                .Where(loadedPath => loadedPath == path)
                .Take(1)
                .Select(_ => assetProvider.GetAsset<T>(path));
        }
        
        public static async UniTask PreloadAssets(this IAssetProvider assetProvider, IEnumerable<string> paths)
        {
            var tasks = ListPool<UniTask>.Get();

            foreach (var path in paths)
            {
                tasks.Add(assetProvider.PreloadAsset<Object>(path));
            }
            
            await UniTask.WhenAll(tasks);
        }
    }
}