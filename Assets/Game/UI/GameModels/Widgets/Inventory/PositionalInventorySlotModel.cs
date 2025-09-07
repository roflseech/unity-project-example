using System;
using Game.Models.InventoryManagement;
using UniRx;

namespace Game.UI.GameModels.Widgets.Inventory
{
    public interface IPositionalInventorySlotModel
    {
        IObservable<string> ImagePath { get; }
        IObservable<int> Quantity { get; }
        int Slot { get; }
    }
    
    public class PositionalInventorySlotModel : IPositionalInventorySlotModel
    {
        private readonly int _slot;
        private readonly IPositionalInventory _inventory;
        private readonly IInventoryVisualDataProvider _inventoryVisualDataProvider;
        
        public PositionalInventorySlotModel(int slot, IPositionalInventory inventory, 
            IInventoryVisualDataProvider inventoryVisualDataProvider)
        {
            _slot = slot;
            _inventory = inventory;
            _inventoryVisualDataProvider = inventoryVisualDataProvider;
        }

        public IObservable<string> ImagePath => OnSlotUpdated.StartWith(Unit.Default)
            .Select(_ =>
            {
                 if (_inventory.IsSlotEmpty(_slot))
                 {
                     return string.Empty;
                 }

                 var itemId = _inventory.GetSlot(_slot).ItemId;
                 return _inventoryVisualDataProvider.GetItemVisualData(itemId).IconPath;
            });
        
        public IObservable<int> Quantity => OnSlotUpdated.StartWith(Unit.Default)
            .Select(_ => _inventory.GetSlot(_slot).Quantity);
        
        public int Slot => _slot;
        
        private IObservable<Unit> OnSlotUpdated => _inventory.OnSlotUpdate.Where(x => x == _slot).AsUnitObservable();
    }
}