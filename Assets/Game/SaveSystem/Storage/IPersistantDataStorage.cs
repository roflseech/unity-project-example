using System;
using System.Buffers;
using Cysharp.Threading.Tasks;

namespace Game.SaveSystem.Storage
{
    internal interface IPersistantDataStorage
    {
        UniTask SaveAsync(string key, ArrayBufferWriter<byte> bufferWriter);
        UniTask LoadAsync(string key, ArrayBufferWriter<byte> bufferWriter);
        bool HasSaved(string key);
    }
}