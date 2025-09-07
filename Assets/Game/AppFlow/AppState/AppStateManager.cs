using Cysharp.Threading.Tasks;
using VContainer;

namespace Game.AppFlow.AppState
{
    public interface IAppStateManager
    {
        bool GoToState<TState>() where TState : class, IAppState;
    }

    public class AppStateManager : IAppStateManager
    {
        private readonly IObjectResolver _objectResolver;

        private IAppState _currentState;
        private bool _transition;
    
        public AppStateManager(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public bool GoToState<TState>() where TState : class, IAppState
        {
            if (_transition) return false;
        
            GoToStateAsync<TState>().Forget();
            return true;
        }

        private async UniTask GoToStateAsync<TState>() where TState : class, IAppState
        {
            _transition = true;
            if (_currentState != null) await _currentState.ExitAsync();
        
            _currentState = _objectResolver.Resolve<TState>();
            await _currentState.EnterAsync();
            _transition = false;
        }
    }

}