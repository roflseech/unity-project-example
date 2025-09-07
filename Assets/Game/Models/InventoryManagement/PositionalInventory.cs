using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Mathematics;

namespace Game.Models.InventoryManagement
{
    public class PositionalInventory : IPositionalInventory
    {
        private readonly List<ItemEntry> _slots;
        private readonly int _maxStack;

        private readonly Subject<int> _onUpdate = new();
        
        public IReadOnlyList<ItemEntry> Slots => _slots;
        public IObservable<int> OnSlotUpdate => _onUpdate;

        public PositionalInventory(int capacity, int maxStack)
        {
            _maxStack = maxStack;
            _slots = new List<ItemEntry>(capacity);
            _slots.AddRange(Enumerable.Repeat(new ItemEntry(null, 0), capacity));
        }
        
        public int SlotsCount => _slots.Count;

        public ItemEntry GetSlot(int slotIndex)
        {
            return _slots[slotIndex];
        }

        public void ReplaceSlot(int slotIndex, ItemEntry item)
        {
            _slots[slotIndex] = item;
            _onUpdate.OnNext(slotIndex);
        }
        
        public bool AddItem(ItemEntry item)
        {
            var freeSpace = SpaceForItem(item.Item);

            if (freeSpace < item.Quantity)
            {
                return false;
            }

            var itemsLeft = item.Quantity;
            
            for (int i = 0; i < _slots.Count && itemsLeft > 0; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    var spaceToUse = math.min(_maxStack, itemsLeft);
                    itemsLeft -= spaceToUse;
                    ReplaceSlot(i, new ItemEntry(item.Item, spaceToUse));
                }
                else if (CanStack(_slots[i].Item, item.Item))
                {
                    var slotFreeSpace = _maxStack - _slots[i].Quantity;
                    var spaceToUse = math.min(slotFreeSpace, itemsLeft);
                    itemsLeft -= spaceToUse;
                    var finalSlotQuantity = _slots[i].Quantity + spaceToUse;
                    ReplaceSlot(i, new ItemEntry(item.Item, finalSlotQuantity));
                }
            }

            return true;
        }

        public int SpaceForItem(IItem item)
        {
            int totalSpace = 0;
            
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].IsEmpty)
                {
                    totalSpace += _maxStack;
                }
                else
                {
                    var spaceInSlot = CanStack(_slots[i].Item, item) ? _maxStack - _slots[i].Quantity : 0;
                    totalSpace += spaceInSlot;
                }
            }
            
            return totalSpace;
        }

        // Legacy methods for backward compatibility
        public int SpaceForItem(int itemId)
        {
            return SpaceForItem(new BaseItem(itemId));
        }

        public bool AddItem(int itemId, int quantity)
        {
            return AddItem(new ItemEntry(new BaseItem(itemId), quantity));
        }
        
        public bool IsSlotEmpty(int slot)
        {
            return _slots[slot].IsEmpty;
        }

        public bool HasSlot(int slot)
        {
            return slot >= 0 && slot < SlotsCount;
        }

        private bool CanStack(IItem item1, IItem item2)
        {
            if (item1 == null || item2 == null)
                return false;
                
            // BaseItems can stack if they have the same ID
            if (item1 is BaseItem && item2 is BaseItem)
                return item1.Id == item2.Id;
                
            // StatefulItems cannot stack (they have unique states)
            return false;
        }
    }
}