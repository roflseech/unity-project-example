using Cysharp.Threading.Tasks;

namespace Game.AppFlow.AppState
{
    public interface IAppState
    {
        UniTask EnterAsync();
        UniTask ExitAsync();
    }
}