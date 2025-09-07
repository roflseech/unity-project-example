using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Provider
{
    public class UiScene : MonoBehaviour, IUiScene
    {
        [SerializeField]
        private RectTransform _container;

        public void AttachTransform(Transform windowTransform)
        {
            windowTransform.SetParent(_container, false);
            windowTransform.localPosition = Vector3.zero;
            windowTransform.localScale = Vector3.one;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container);
        }
    }
}