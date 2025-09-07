using System;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using UnityEngine;

namespace Game.JsBridge
{
    public class JsExecutor : IJsExecutor, IDisposable
    {
        private Engine _jsEngine;
        
        public void Initialize(IEnumerable<string> files)
        {
            if (_jsEngine != null)
            {
                throw new Exception("Already initialized");
            }
            
            _jsEngine = new();
            foreach (var file in files)
            {
                _jsEngine.Execute(file);
            }
        }
        
        public TResult Execute<TResult>(string funcGetter, object[] args, Func<JsValue, TResult> converter)
        {
            return converter.Invoke(_jsEngine.Invoke(funcGetter, args));
        }

        public void Dispose()
        {
            _jsEngine?.Dispose();
        }
    }
}