using Game.UI.GameModels.Widgets;
using Game.UI.Presenters;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GamePresenters.Widgets
{
    public class ButtonWidget : BaseWidget<IButtonWidgetModel>
    {
        [SerializeField] private Button _clickZone;
        
        protected override void SetBindings(IButtonWidgetModel model, CompositeDisposable bindings)
        {
            _clickZone.OnClickAsObservable()
                .Subscribe(_ => model.Click())
                .AddTo(bindings);
        }
    }
}