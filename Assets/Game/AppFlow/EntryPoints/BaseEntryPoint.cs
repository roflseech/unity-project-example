using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Game.AppFlow.EntryPoints
{
    public abstract class BaseEntryPoint : IAsyncStartable
    {
        private readonly ICoreInitializer _coreInitializer;

        protected BaseEntryPoint(ICoreInitializer coreInitializer)
        {
            _coreInitializer = coreInitializer;
        }

        protected abstract UniTask InitializeAsync();
        
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await _coreInitializer.InitializeIfNeededAsync();
            await InitializeAsync();
        }
    }
}