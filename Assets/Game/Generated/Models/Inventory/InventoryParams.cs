using System;
using System.Linq;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;
using Game.JsBridge;

namespace Game.Generated.Models.Inventory
{
    [TsGenerated]
    public readonly struct InventoryParams
    {
        public readonly int StackSize;

        public InventoryParams(
            int stackSize
        )
        {
            StackSize = stackSize;
        }
    }

    public static class InventoryParamsConverter
    {
        public static InventoryParams AsInventoryParams(this JsValue jsValue)
        {
            var tmpStackSize = jsValue.Get("stackSize");
            var stackSize = Game.JsBridge.BaseJsConverters.AsInt(tmpStackSize);

            return new InventoryParams(stackSize);
        }
    }
}
