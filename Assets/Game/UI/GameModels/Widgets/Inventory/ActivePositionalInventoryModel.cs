using System.Collections.Generic;
using Game.Models.InventoryManagement;


namespace Game.UI.GameModels.Widgets.Inventory
{
    public interface IActivePositionalInventoryModel
    {
        IReadOnlyList<IActiveInventorySlotModel> Slots { get; }
    }
    
    public class ActivePositionalInventoryModel : IActivePositionalInventoryModel
    {
        private readonly IPlayerActiveInventory _inventory;
        private readonly IInventoryVisualDataProvider _inventoryVisualDataProvider;
        
        private List<ActiveInventorySlotModel> _slots;

        public ActivePositionalInventoryModel(IPlayerActiveInventory inventory, IInventoryVisualDataProvider inventoryVisualDataProvider)
        {
            _inventory = inventory;
            _inventoryVisualDataProvider = inventoryVisualDataProvider;
        }

        public IReadOnlyList<IActiveInventorySlotModel> Slots
        {
            get
            {
                if (_slots == null)
                {
                    _slots = new();
                    for (int i = 0; i < _inventory.Inventory.SlotsCount; i++)
                    {
                        _slots.Add(new ActiveInventorySlotModel(_inventory, _inventoryVisualDataProvider, i));
                    }
                }

                return _slots;
            }
        }
    }
}