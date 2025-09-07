
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;

namespace Game.Generated.Models.Inventory
{
    [TsGenerated]
    public readonly struct ItemVisualData
    {
        public readonly string IconPath;

        public ItemVisualData(
            string iconPath
        )
        {
            IconPath = iconPath;
        }
    }

    public static class ItemVisualDataConverter
    {
        public static ItemVisualData AsItemVisualData(this JsValue jsValue)
        {
            var tmpIconPath = jsValue.Get("iconPath");
            var iconPath = Game.JsBridge.BaseJsConverters.AsString(tmpIconPath);

            return new ItemVisualData(iconPath);
        }
    }
}
