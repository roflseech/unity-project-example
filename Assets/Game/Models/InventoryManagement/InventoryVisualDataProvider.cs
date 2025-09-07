using Game.Generated.Models.Inventory;
using Game.Generated.Query.Inventory;
using Game.JsBridge;
using UnityEngine;

namespace Game.Models.InventoryManagement
{
    public interface IInventoryVisualDataProvider
    {
        ItemVisualData GetItemVisualData(int itemId);
    }
    
    public class InventoryVisualDataProvider : IInventoryVisualDataProvider
    {
        private readonly IQueryProvider _queryProvider;

        public InventoryVisualDataProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public ItemVisualData GetItemVisualData(int itemId)
        {
            var visualData = _queryProvider.Get<InventoryQuery.GetItemVisualDataById>().Execute(itemId);
            
            if (visualData != null) return visualData.Value;

            Debug.LogError($"Couldn't find item visual for id {itemId}");
            return new ItemVisualData(string.Empty);
        }
    }
}