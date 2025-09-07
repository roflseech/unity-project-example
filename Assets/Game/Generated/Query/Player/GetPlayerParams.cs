using System;
using System.Linq;
using Game.JsBridge;
using Game.TsClassCodeGenerator.Runtime;
using Jint.Native;
using Jint;
using Game.Generated.Models.Player;

namespace Game.Generated.Query.Player
{
    public partial class PlayerQuery
    {
        [TsGeneratedFunc]
        public class GetPlayerParams : BaseQuery
        {

            public PlayerParams Execute()
            {
                return ExecuteJsFunction<PlayerParams>("getPlayerParams", new object[] {  }, x => x.AsPlayerParams());
            }
        }
    }
}
