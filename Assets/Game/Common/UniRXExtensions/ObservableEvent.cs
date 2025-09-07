using System;
using UniRx;

namespace Game.Common.UniRXExtensions
{
    public class ObservableEvent : IObservable<Unit>
    {
        private event Action<Unit> _action;
       
        public void Invoke()
        {
            _action?.Invoke(Unit.Default);
        }
        
        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            return AsObservable().Subscribe(observer);
        }
        
        private IObservable<Unit> AsObservable() => Observable.FromEvent<Unit>(
            e => _action += e,
            e => _action -= e);
    }
    
    public class ObservableEvent<T> : IObservable<T>
    {
        private event Action<T> _action;
       
        public void Invoke(T arg)
        {
            _action?.Invoke(arg);
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return AsObservable().Subscribe(observer);
        }
        
        private IObservable<T> AsObservable() => Observable.FromEvent<T>(
            e => _action += e,
            e => _action -= e);
    }
}