using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Game.AppFlow.AppState
{
    public abstract class BaseAppState : IAppState, IDisposable
    {
        private readonly CompositeDisposable _activeStateDisposables = new();
        private readonly IAppStateManager _stateManager;
        
        protected void GoToState<T>() where T : class, IAppState
        {
            _stateManager.GoToState<T>();
        }
        
        protected abstract UniTask StateEnterAsync(CompositeDisposable activeDisposables);
        protected abstract UniTask StateExitAsync();
    
        public UniTask EnterAsync()
        {
            return StateEnterAsync(_activeStateDisposables);
        }

        public virtual UniTask ExitAsync()
        {
            _activeStateDisposables.Clear();
            return StateExitAsync();
        }

        public void Dispose()
        {
            _activeStateDisposables.Dispose();
        }
    }
}