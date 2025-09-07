using System;
using Game.Common.UniRXExtensions;
using UniRx;

namespace Game.UI.GameModels.Widgets
{
    public interface IButtonWidgetModel
    {
        void Click();
    }
    
    public class ButtonWidgetModel : IButtonWidgetModel
    {
        private readonly ObservableEvent _onClick = new();
        private readonly Action _clickAction;

        public ButtonWidgetModel(Action clickAction)
        {
            _clickAction = clickAction;
        }
        
        public void Click()
        {
            _clickAction?.Invoke();
        }
    }
}