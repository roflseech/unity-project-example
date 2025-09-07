using System;
using Game.Models.InventoryManagement;
using UniRx;


namespace Game.UI.GameModels.Widgets.Inventory
{
    public interface IActiveInventorySlotModel
    {
        IPositionalInventorySlotModel Slot { get; }
        IObservable<bool> IsSelected { get; }
    }
    
    public class ActiveInventorySlotModel : IActiveInventorySlotModel
    {
        private readonly IPlayerActiveInventory _inventory;
        private readonly IInventoryVisualDataProvider _inventoryVisualDataProvider;
        
        private readonly int _slot;

        private IPositionalInventorySlotModel _slotModel;

        public IPositionalInventorySlotModel Slot => _slotModel ??= new PositionalInventorySlotModel(_slot, _inventory.Inventory, _inventoryVisualDataProvider);
        public IObservable<bool> IsSelected => _inventory.Selection.SelectedSlot.Select(x => x == _slot);

        public ActiveInventorySlotModel(IPlayerActiveInventory inventory, IInventoryVisualDataProvider inventoryVisualDataProvider, int slot)
        {
            _inventory = inventory;
            _inventoryVisualDataProvider = inventoryVisualDataProvider;
            _slot = slot;
        }
    }
}