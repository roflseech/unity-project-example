
namespace Game.Models.InventoryManagement
{
    public interface IPlayerActiveInventory
    {
        IPositionalInventory Inventory { get; }
        IPositionalInventorySelection Selection { get; }
    }
    
    public class PlayerActiveInventory : IPlayerActiveInventory
    {
        private readonly PositionalInventory _inventory;
        private readonly PositionalInventorySelection _selection;
        
        public IPositionalInventory Inventory => _inventory;
        public IPositionalInventorySelection Selection => _selection;

        public PlayerActiveInventory(int slotCount, int maxStack)
        {
            _inventory = new(slotCount, maxStack);
            _selection = new(_inventory);
        }
    }
}