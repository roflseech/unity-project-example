using System;
using System.Linq;
using Game.JsBridge;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;
using Game.Generated.Models.Inventory;

namespace Game.Generated.Query.Inventory
{
    public partial class InventoryQuery
    {
        [TsGeneratedFunc]
        public class GetItemById : BaseQuery
        {

            public ItemDefinition? Execute(int id)
            {
                return ExecuteJsFunction<ItemDefinition?>("getItemById", new object[] { id }, x => x.IsUndefined() ? default : x.AsItemDefinition());
            }
        }
    }
}
