using Game.UI.GameModels.Widgets.Inventory;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;

namespace Game.UI.GamePresenters.Widgets.Inventory
{
    public class ActiveInventorySlotWidget : BaseWidget<IActiveInventorySlotModel>
    {
        [SerializeField] private PositionalInventorySlotWidget _slotWidget;
        [SerializeField] private GameObject _selection;
        
        protected override void SetBindings(IActiveInventorySlotModel model, CompositeDisposable bindings)
        {
            _slotWidget.Bind(model.Slot);
            model.IsSelected
                .Subscribe(isSelected => _selection.SetActive(isSelected))
                .AddTo(bindings);
        }
    }
}