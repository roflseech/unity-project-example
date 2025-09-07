using Game.UI.GameModels.Widgets;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GamePresenters.Widgets
{
    public class ButtonWithTextWidget : BaseWidget<IButtonWithTextWidgetModel>
    {
        [SerializeField] private TextWidget _textWidget;
        [SerializeField] private Button _clickArea;
        
        protected override void SetBindings(IButtonWithTextWidgetModel model, CompositeDisposable bindings)
        {
            _textWidget.Bind(model.Text);
            
            _clickArea.OnClickAsObservable()
                .Subscribe(_ => model.Click())
                .AddTo(bindings);
        }
    }
}