using Game.Generated.Query.Player;
using Game.JsBridge;
using Game.Models.InventoryManagement;
using Game.Models.Query;

namespace Game.Gameplay.Models.Player
{
    public interface IPlayerData
    {
        IPlayerActiveInventory Inventory { get; }
    }
    
    public class PlayerDataModel : IPlayerData
    {
        private readonly IQueryProvider _queryProvider;
        private readonly IInventoryParamsProvider _inventoryParamsProvider;

        private PlayerActiveInventory _inventory;

        public PlayerDataModel(IInventoryParamsProvider inventoryParamsProvider, IQueryProvider queryProvider)
        {
            _inventoryParamsProvider = inventoryParamsProvider;
            _queryProvider = queryProvider;
        }

        public IPlayerActiveInventory Inventory
        {
            get
            {
                var playerParams = _queryProvider.Get<PlayerQuery.GetPlayerParams>().Execute();
                
                return _inventory ??= new PlayerActiveInventory(
                    playerParams.InventorySize,
                    _inventoryParamsProvider.MaxStack);
            }
        }
    }
}