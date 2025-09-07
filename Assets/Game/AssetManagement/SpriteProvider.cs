using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

namespace Game.AssetManagement
{
    public class SpriteProvider : ISpriteProvider
    {
        private readonly IOnDemandAssetProvider _assetProvider;

        public SpriteProvider(IAssetProvider assetProvider)
        {
            _assetProvider = new OnDemandAssetProvider(assetProvider);
        }

        public IObservable<Sprite> GetSpriteAsObservable(string path)
        {
            return _assetProvider.GetAssetAsObservable<Sprite>(path);
        }
    }
}