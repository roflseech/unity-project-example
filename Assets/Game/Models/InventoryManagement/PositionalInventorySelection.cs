using UniRx;
using UnityEngine;

namespace Game.Models.InventoryManagement
{
    public interface IPositionalInventorySelection
    {
        IReadOnlyReactiveProperty<int> SelectedSlot { get; }
        void SelectSlot(int slot);
    }
    
    public class PositionalInventorySelection : IPositionalInventorySelection
    {
        private readonly ReactiveProperty<int> _selectedSlot;
        private readonly IPositionalInventory _positionalInventory;
        
        public IReadOnlyReactiveProperty<int> SelectedSlot => _selectedSlot;
        
        public void SelectSlot(int slot)
        {
            if (!_positionalInventory.HasSlot(slot))
            {
                Debug.LogError($"Attempt to select slot out of bounds {slot}. Slot count: {_positionalInventory.SlotsCount}.");
                return;
            }
            _selectedSlot.Value = slot;
        }

        public PositionalInventorySelection(IPositionalInventory positionalInventory)
        {
            _positionalInventory = positionalInventory;
            _selectedSlot = new(0);
        }
    }
}