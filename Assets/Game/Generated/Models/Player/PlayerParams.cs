using System;
using System.Linq;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;
using Game.JsBridge;

namespace Game.Generated.Models.Player
{
    [TsGenerated]
    public readonly struct PlayerParams
    {
        public readonly float Speed;
        public readonly int InventorySize;

        public PlayerParams(
            float speed,
            int inventorySize
        )
        {
            Speed = speed;
            InventorySize = inventorySize;
        }
    }

    public static class PlayerParamsConverter
    {
        public static PlayerParams AsPlayerParams(this JsValue jsValue)
        {
            var tmpSpeed = jsValue.Get("speed");
            var speed = Game.JsBridge.BaseJsConverters.AsFloat(tmpSpeed);
            var tmpInventorySize = jsValue.Get("inventorySize");
            var inventorySize = Game.JsBridge.BaseJsConverters.AsInt(tmpInventorySize);

            return new PlayerParams(speed, inventorySize);
        }
    }
}
