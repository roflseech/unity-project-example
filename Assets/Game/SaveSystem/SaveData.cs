using System;
using Game.Common.UniRXExtensions;
using UniRx;

namespace Game.SaveSystem
{
    internal class SaveData<T> : ISaveData<T>
    {
        private readonly ISaveDataManager _saveDataManager;
        
        private readonly ObservableValue<T> _value = new();
        private readonly string _key;

        public string Key => _key;
        public T Value => _value.Value;
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _value.Skip(1).Subscribe(observer);
        }
        
        public void Publish(T value)
        {
            _value.Value = value;
        }

        public void Modify(Func<T, T> modifier)
        {
            _value.Value = modifier(_value.Value);
        }
    }
}