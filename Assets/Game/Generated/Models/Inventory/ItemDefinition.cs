using System;
using System.Linq;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;
using Game.JsBridge;
using Game.Generated.Models.Inventory;

namespace Game.Generated.Models.Inventory
{
    [TsGenerated]
    public readonly struct ItemDefinition
    {
        public readonly ItemVisualData VisualData;
        public readonly bool? Stackable;

        public ItemDefinition(
            ItemVisualData visualData,
            bool? stackable
        )
        {
            VisualData = visualData;
            Stackable = stackable;
        }
    }

    public static class ItemDefinitionConverter
    {
        public static ItemDefinition AsItemDefinition(this JsValue jsValue)
        {
            var tmpVisualData = jsValue.Get("visualData");
            var visualData = tmpVisualData.AsItemVisualData();
            var tmpStackable = jsValue.Get("stackable");
            var stackable = tmpStackable.IsUndefined() ? (bool?)null : Game.JsBridge.BaseJsConverters.AsBoolean(tmpStackable);

            return new ItemDefinition(visualData, stackable);
        }
    }
}
