using Game.UI.Common;
using Game.UI.Common.Inject;
using Game.UI.Models.Window;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;

namespace Game.UI.Presenters.Window
{
    public abstract class BaseWindow<T> : BaseWidget<T>, IWindow<T> where T : class, IWindowModel
    {
        public Transform Transform => transform;
        
        protected abstract void OnWindowOpen();
        protected abstract void OnWindowClose();

        public override void Bind(T model)
        {
            if (model == null)
            {
                Unbind();
                return;
            }
            
            base.Bind(model);
        }

        public void OnOpen()
        {
            OnWindowOpen();
            Model.OnOpen();
        }

        public void OnClose()
        {
            OnWindowClose();
            Model.OnClose();
        }
    }
}