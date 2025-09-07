using System;
using Game.Generated.Query.Inventory;
using Game.JsBridge;

namespace Game.Models.InventoryManagement
{
    public interface IInventoryParamsProvider
    {
        int MaxStack { get; }
    }
    
    public class InventoryParamsProvider : IInventoryParamsProvider
    {
        private readonly IQueryProvider _queryProvider;
        private readonly Lazy<int> _maxStack;
        
        public int MaxStack => _maxStack.Value;

        public InventoryParamsProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
            
            _maxStack = new Lazy<int>(() => _queryProvider.Get<InventoryQuery.GetInventoryParams>().Execute().StackSize);
        }
    }
}