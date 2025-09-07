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
        public class GetItemVisualDataById : BaseQuery
        {

            public ItemVisualData? Execute(int id)
            {
                return ExecuteJsFunction<ItemVisualData?>("getItemVisualDataById", new object[] { id }, x => x.IsUndefined() ? default : x.AsItemVisualData());
            }
        }
    }
}
