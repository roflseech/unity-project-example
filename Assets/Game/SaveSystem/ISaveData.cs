using System;

namespace Game.SaveSystem
{
    public interface IReadOnlySaveData<out T> : IObservable<T>
    {
        string Key { get; }
        T Value { get; }
    }
    
    public interface ISaveData<T> : IReadOnlySaveData<T>
    {
        void Publish(T value);
        void Modify(Func<T, T> modifier);
    }
}