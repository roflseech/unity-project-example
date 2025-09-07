using Game.Gameplay.Models;

namespace Game.Models.InventoryManagement
{
    public readonly struct ItemEntry
    {
        public readonly IItem Item;
        public readonly int Quantity;

        public ItemEntry(IItem item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
        
        public bool IsEmpty => Item == null;
        public int ItemId => Item?.Id ?? ModelConstants.EMPTY_ID;
    }
}