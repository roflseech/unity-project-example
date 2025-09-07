namespace Game.Models.InventoryManagement
{
    public readonly struct ItemState
    {
        public readonly int Id;
        public readonly float Value;

        public ItemState(int id, float value)
        {
            Id = id;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemState other && Id == other.Id && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(Id, Value);
        }

        public override string ToString()
        {
            return $"ItemState(Id={Id}, Value={Value})";
        }
    }
}