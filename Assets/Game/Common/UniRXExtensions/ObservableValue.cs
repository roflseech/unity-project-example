using System;
using System.Collections.Generic;

namespace Game.Common.UniRXExtensions
{
    public interface IReadOnlyObservableValue<out T> : IObservable<T>
    {
        T Value { get; }
    }
    
    public interface IObservableValue<T> : IReadOnlyObservableValue<T>
    {
        new T Value { get; set; }
    }
    
    public class ObservableValue<T> : IObservableValue<T>
    {
        private readonly ObservableEvent<T> _event = new();
        
        private T _value;

        public ObservableValue()
        {
            
        }

        public ObservableValue(T value)
        {
            _value = value;
        }
        
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;
                
                _value = value;
                _event.Invoke(_value);
            }
        }
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(Value);
            return _event.Subscribe(observer);
        }
    }
}