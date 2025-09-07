using Game.UI.GameLayers;

namespace Game.UI.GameModels.Windows
{
    public interface IWindowFactory
    {

    }
    
    public class WindowFactory : IWindowFactory
    {
        private readonly IUiAggregate _uiAggregate;

        public WindowFactory(IUiAggregate uiAggregate)
        {
            _uiAggregate = uiAggregate;
        }
        
    }
}