using Game.UI.Common.UiElements;
using Game.UI.GameModels.Windows;
using Game.UI.GamePresenters.Widgets;
using Game.UI.Presenters.Window;
using UniRx;
using UnityEngine;

namespace Game.UI.GamePresenters.Windows
{
    public class HomeWindow : BaseWindow<IHomeWindowModel>
    {
        [SerializeField] private ButtonWithTextWidget _startGameButton;
        [SerializeField] private ButtonWithTextWidget _settingsButton;
        
        protected override void SetBindings(IHomeWindowModel model, CompositeDisposable bindings)
        {
            _startGameButton.Bind(model.StartGameButton);
            _settingsButton.Bind(model.SettingsButton);
        }

        protected override void OnWindowOpen()
        {
            
        }

        protected override void OnWindowClose()
        {
            
        }
    }
}