using System;
using UniRx;

namespace Game.UI.GameModels.Widgets
{
    public interface IButtonWithTextWidgetModel : IButtonWidgetModel
    {
        ITextWidgetModel Text { get; }
    }
    
    public class ButtonWithTextWidgetModel : ButtonWidgetModel, IButtonWithTextWidgetModel
    {
        public ITextWidgetModel Text { get; }

        public ButtonWithTextWidgetModel(ITextWidgetModel textModel, Action clickAction) : base(clickAction)
        {
            Text = textModel;
        }
    }
}