using Game.SaveSystem;
using Game.State;
using Game.UI.GameLayers;
using Game.UI.GameModels.Widgets;
using Game.UI.Models.Window;

namespace Game.UI.GameModels.Windows
{
    public interface IHomeWindowModel : IWindowModel
    {
        IButtonWithTextWidgetModel StartGameButton { get; }
        IButtonWithTextWidgetModel SettingsButton { get; }
    }
    
    public class HomeWindowModel : IHomeWindowModel
    {
        private const string START_GAME_BUTTON = "start_game_button";
        private const string SETTINGS_BUTTON = "settings_button";
        
        private readonly IUiAggregate _uiAggregate;

        private IButtonWithTextWidgetModel _startGameButton;
        private IButtonWithTextWidgetModel _settingsButton;

        public IButtonWithTextWidgetModel StartGameButton => _startGameButton ??= 
            new ButtonWithTextWidgetModel(
                new TextWidgetModel(START_GAME_BUTTON, true),
                () =>
                {
                    //_uiAggregate.Get(UiLayer.Main).OpenSingletonWindow<IBaseGameplayWindowModel>();
                });
        
        public IButtonWithTextWidgetModel SettingsButton => _settingsButton ??= 
            new ButtonWithTextWidgetModel(
                new TextWidgetModel(SETTINGS_BUTTON, true),
                () =>
                {
                    //_uiAggregate.Get(UiLayer.Main).OpenSingletonWindow<IBaseGameplayWindowModel>();
                });
        
        public HomeWindowModel( IUiAggregate uiAggregate)
        {
            _uiAggregate = uiAggregate;
        }
        
        public void OnOpen()
        {
        }

        public void OnClose()
        {
        }
    }
}