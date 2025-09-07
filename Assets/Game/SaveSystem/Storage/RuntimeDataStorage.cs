using System;
using System.Buffers;
using System.Collections.Generic;
using MemoryPack;

namespace Game.SaveSystem.Storage
{
    internal interface IRuntimeDataStorage
    {
        void SetRaw(string key, ReadOnlySpan<byte> value);
        void Set<T>(string key, T value);
        T Get<T>(string key);
        bool HasSaved(string key);
        ArrayBufferWriter<byte> GetWriter(string key);
    }
    
    internal class RuntimeDataStorage : IRuntimeDataStorage
    {
        private readonly Dictionary<string, ArrayBufferWriter<byte>> _cachedData = new();

        public void SetRaw(string key, ReadOnlySpan<byte> value)
        {
            var bw = GetWriter(key);
            bw.Clear();
            bw.Write(value);
        }

        public void Set<T>(string key, T value)
        {
            var bw = GetWriter(key);
            bw.Clear();
            MemoryPackSerializer.Serialize(bw, value);
        }

        public T Get<T>(string key)
        {
            var bw = GetWriter(key);
            return MemoryPackSerializer.Deserialize<T>(bw.WrittenSpan);
        }

        public bool HasSaved(string key)
        {
            return _cachedData.ContainsKey(key);
        }

        public ArrayBufferWriter<byte> GetWriter(string key)
        {
            if (!_cachedData.TryGetValue(key, out var writer))
            {
                writer = new ArrayBufferWriter<byte>();
                _cachedData[key] = writer;
            }
            
            return writer;
        }
    }
}