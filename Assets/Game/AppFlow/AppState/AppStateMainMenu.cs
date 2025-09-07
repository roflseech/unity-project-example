using Cysharp.Threading.Tasks;
using Game.UI.GameLayers;
using Game.UI.GameModels.Windows;
using UniRx;

namespace Game.AppFlow.AppState
{
    public class AppStateMainMenu : BaseAppState
    {
        private readonly IUiAggregate _uiAggregate;

        public AppStateMainMenu(IUiAggregate uiAggregate)
        {
            _uiAggregate = uiAggregate;
        }

        protected override async UniTask StateEnterAsync(CompositeDisposable activeDisposables)
        {
            await _uiAggregate.Get(UiLayer.Main).PreloadAllWindows();
            await _uiAggregate.Get(UiLayer.Popup).PreloadAllWindows();
            _uiAggregate.Get(UiLayer.Main).OpenSingletonWindow<IHomeWindowModel>();
        }

        protected override UniTask StateExitAsync()
        {
            _uiAggregate.Get(UiLayer.Main).UnloadAll();
            _uiAggregate.Get(UiLayer.Popup).UnloadAll();
            return UniTask.CompletedTask;
        }
    }
}