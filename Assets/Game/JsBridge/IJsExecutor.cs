using System;
using System.Collections.Generic;
using Jint.Native;

namespace Game.JsBridge
{
    public interface IJsExecutor
    {
        void Initialize(IEnumerable<string> files);
        TResult Execute<TResult>(string funcGetter, object[] args, Func<JsValue, TResult> converter);
    }
}