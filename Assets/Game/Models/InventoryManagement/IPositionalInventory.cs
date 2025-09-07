using System;
using System.Collections.Generic;

namespace Game.Models.InventoryManagement
{
    public interface IPositionalInventory
    {
        IReadOnlyList<ItemEntry> Slots { get; }
        int SlotsCount { get; }
        ItemEntry GetSlot(int slotIndex);
        void ReplaceSlot(int slotIndex, ItemEntry item);
        bool AddItem(ItemEntry item);
        int SpaceForItem(IItem item);
        IObservable<int> OnSlotUpdate { get; }
        bool IsSlotEmpty(int slot);
        bool HasSlot(int slot);
        
        // Legacy methods for backward compatibility
        int SpaceForItem(int itemId);
        bool AddItem(int itemId, int quantity);
    }
}