using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Observable = UniRx.Observable;

namespace Game.AssetManagement
{
    public interface IOnDemandAssetProvider
    {
        IObservable<T> GetAssetAsObservable<T>(string path) where T : Object;
    }
    
    public class OnDemandAssetProvider : IOnDemandAssetProvider
    {
        private readonly IAssetProvider _assetProvider;
        private readonly Dictionary<string, int> _spriteCounter = new();
        
        public OnDemandAssetProvider(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        
        public IObservable<T> GetAssetAsObservable<T>(string path) where T : Object
        {
            ModifyCounter(path, 1);
            
            return Observable.Create<T>(o =>
            {
                var disposable = new CompositeDisposable();
                
                _assetProvider.LoadAssetAsObservable<T>(path)
                    .Subscribe(o.OnNext).AddTo(disposable);

                Disposable.Create(() =>
                {
                    ModifyCounter(path, -1);
                }).AddTo(disposable);
                
                return disposable;
            });
        }

        private void ModifyCounter(string path, int delta)
        {
            _spriteCounter.TryAdd(path, 0);
            _spriteCounter[path] += delta;

            if (_spriteCounter[path] == 0)
            {
                _assetProvider.UnloadAsset(path);
            }
        }
    }
}