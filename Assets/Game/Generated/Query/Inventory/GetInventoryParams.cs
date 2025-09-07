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
        public class GetInventoryParams : BaseQuery
        {

            public InventoryParams Execute()
            {
                return ExecuteJsFunction<InventoryParams>("getInventoryParams", new object[] {  }, x => x.AsInventoryParams());
            }
        }
    }
}
