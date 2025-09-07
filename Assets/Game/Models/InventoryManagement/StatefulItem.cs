using System.Collections.Generic;
using System.Linq;

namespace Game.Models.InventoryManagement
{
    /// <summary>
    /// Parameter of an item
    /// </summary>
    public sealed class StatefulItem : IItem
    {
        public int Id { get; }
        public List<ItemState> States { get; }

        public StatefulItem(int id, List<ItemState> states = null)
        {
            Id = id;
            States = states ?? new List<ItemState>();
        }

        public StatefulItem(int id, params ItemState[] states)
        {
            Id = id;
            States = states?.ToList() ?? new List<ItemState>();
        }

        public void AddState(ItemState state)
        {
            States.Add(state);
        }

        public void RemoveState(int stateId)
        {
            States.RemoveAll(s => s.Id == stateId);
        }

        public ItemState? GetState(int stateId)
        {
            return States.FirstOrDefault(s => s.Id == stateId);
        }

        public void UpdateState(int stateId, float newValue)
        {
            for (int i = 0; i < States.Count; i++)
            {
                if (States[i].Id == stateId)
                {
                    States[i] = new ItemState(stateId, newValue);
                    return;
                }
            }
            
            States.Add(new ItemState(stateId, newValue));
        }

        public override bool Equals(object obj)
        {
            return obj is StatefulItem other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"StatefulItem(Id={Id}, States={States.Count})";
        }
    }
}