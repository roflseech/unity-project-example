using System;
using System.Linq;
using Game.JsBridge;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;

namespace Game.Generated.Query.Inventory
{
    public partial class InventoryQuery
    {
        [TsGeneratedFunc]
        public class GetAllItemIds : BaseQuery
        {

            public int[] Execute()
            {
                return ExecuteJsFunction<int[]>("getAllItemIds", new object[] {  }, x => x.AsArray().Select(v => v.AsInt()).ToArray());
            }
        }
    }
}
