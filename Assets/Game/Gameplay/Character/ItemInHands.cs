using Game.Gameplay.Entity;
using UnityEngine;

namespace Game.Gameplay.Character
{
    public abstract class ItemInHands : MonoBehaviour
    {
        protected CharacterVisual CharacterVisual;

        protected abstract void OnCreated();
        protected abstract void OnDestroyed();
        
        public void Setup(CharacterVisual characterVisual)
        {
            CharacterVisual = characterVisual;
        }
    }
}