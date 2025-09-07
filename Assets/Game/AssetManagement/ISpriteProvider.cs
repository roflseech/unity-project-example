using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.AssetManagement
{
    public interface ISpriteProvider
    {
        IObservable<Sprite> GetSpriteAsObservable(string path);
    }
}