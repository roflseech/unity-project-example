using System;
using System.Collections.Generic;
using Game.UI.Common;
using Game.UI.Common.Inject;
using UniRx;
using UnityEngine;

namespace Game.UI.Presenters.Widget
{
    public abstract class BaseWidget<T> : InjectableUiElement, IBindable<T>
    {
        private readonly CompositeDisposable _bindings = new();

        protected T Model;
        protected bool BindingsReady;
        
        protected abstract void SetBindings(T model, CompositeDisposable bindings);
        
        protected virtual void OnEnable()
        {
            UpdateBindings();
        }

        protected virtual void OnDisable()
        {
            UpdateBindings();
        }
        
        public virtual void Bind(T model)
        {
            if (EqualityComparer<T>.Default.Equals(Model, model))
            {
                return;
            }
            
            BindingsReady = false;
            _bindings.Clear();
            Model = model;

            UpdateBindings();
        }
        
        public virtual void Unbind()
        {
            Model = default;
            BindingsReady = false;
            _bindings.Clear();
        }

        private void UpdateBindings()
        {
            if (Model == null || !gameObject.activeInHierarchy)
            {
                _bindings.Clear();
                BindingsReady = false;
                return;
            }

            if (!BindingsReady)
            {
                SetBindings(Model, _bindings);
                BindingsReady = true;
            }
        }

        protected virtual void OnDestroy()
        {
            _bindings.Dispose();
        }
    }
}