using System;
using Jint.Native;

namespace Game.JsBridge
{
    public abstract class BaseQuery : IQuery
    {
        protected IJsExecutor JsExecutor;
        
        public void Construct(IJsExecutor executor)
        {
            JsExecutor = executor;
        }
        
        protected T ExecuteJsFunction<T>(string functionName, object[] parameters, Func<JsValue, T> converter)
        {
            return JsExecutor.Execute<T>(functionName, parameters, converter);
        }
    }
} 