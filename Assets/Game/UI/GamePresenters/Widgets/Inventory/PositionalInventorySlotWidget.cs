using Cysharp.Text;
using Game.UI.Common.UiElements;
using Game.UI.GameModels.Widgets.Inventory;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;

namespace Game.UI.GamePresenters.Widgets.Inventory
{
    public class PositionalInventorySlotWidget : BaseWidget<IPositionalInventorySlotModel>
    {
        [SerializeField] private TextUiElement _quantityText;
        [SerializeField] private TextUiElement _slotIndexText;
        [SerializeField] private ImageUiElement _iconImage;
        
        protected override void SetBindings(IPositionalInventorySlotModel model, CompositeDisposable bindings)
        {
            model.Quantity.Subscribe(x =>
            {
                if (x > 1)
                {
                    _quantityText.SetInt(x);
                }
                else
                {
                    _quantityText.Clear();
                }
            }).AddTo(bindings);
            
            model.ImagePath.Subscribe(x => _iconImage.SetImage(x)).AddTo(bindings);
            
            _slotIndexText.SetInt(model.Slot + 1);
        }
    }
}