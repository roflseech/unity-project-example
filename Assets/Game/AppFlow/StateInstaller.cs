using Game.SaveSystem;
using Game.State;
using VContainer;

namespace Game.AppFlow
{
    public static class StateInstaller
    {
        private const string PlAYER_STATISTICS_KEY = "player_statistics";
        
        public static void Install(IContainerBuilder builder)
        {
            var saveSystemBuilder = new SaveSystemBuilder(builder);
            
            saveSystemBuilder.Register<PlayerStatistics>(PlAYER_STATISTICS_KEY);
            
            saveSystemBuilder.Build();
        }
        
    }
}