using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.AssetManagement.JsConfigs
{
    public interface IJsConfigLoader
    {
        UniTask<IReadOnlyList<string>> LoadConfigs(string path);
    }
}