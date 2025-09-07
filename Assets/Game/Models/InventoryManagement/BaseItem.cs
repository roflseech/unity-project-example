namespace Game.Models.InventoryManagement
{
    public sealed class BaseItem : IItem
    {
        public int Id { get; }

        public BaseItem(int id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is BaseItem other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"BaseItem(Id={Id})";
        }
    }
}