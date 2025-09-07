using Game.Common.UnityExtensions;
using Game.UI.GameModels.Widgets.Inventory;
using Game.UI.Presenters.Widget;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.GamePresenters.Widgets.Inventory
{
    public class ActivePositionalInventoryWidget : BaseWidget<IActivePositionalInventoryModel>
    {
        [SerializeField] private ActiveInventorySlotWidget _slotPrefab;
        [SerializeField] private Transform _slotContainer;
        
        protected override void SetBindings(IActivePositionalInventoryModel model, CompositeDisposable bindings)
        {
            _slotContainer.ClearObjectsUnderTransform();

            foreach (var slot in model.Slots)
            {
                var widget = AddSlotWidget();
                widget.Bind(slot);
            }
        }

        private ActiveInventorySlotWidget AddSlotWidget()
        {
            var inst = Instantiate(_slotPrefab, _slotContainer, false);
            inst.transform.localScale = Vector3.one;
            inst.transform.localPosition = Vector3.zero;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(inst.GetComponent<RectTransform>());

            return inst;
        }
    }
}