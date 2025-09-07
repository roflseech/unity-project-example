using System;
using System.Collections.Generic;
using Game.JsBridge;

namespace Game.Models.Query
{
    public class QueryProvider : IQueryProvider
    {
        private readonly Dictionary<Type, IQuery> _queryInstances = new();
        private readonly IJsExecutor _jsExecutor;
        
        public QueryProvider(IJsExecutor jsExecutor)
        {
            _jsExecutor = jsExecutor ?? throw new ArgumentNullException(nameof(jsExecutor));
        }
        
        public T Get<T>() where T : IQuery, new()
        {
            var type = typeof(T);
            
            if (_queryInstances.TryGetValue(type, out var cachedInstance))
            {
                return (T)cachedInstance;
            }
            
            try
            {
                var instance = new T();
                instance.Construct(_jsExecutor);
                _queryInstances[type] = instance;
                return instance;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to create query instance of type {type.Name}. Make sure it inherits from BaseQuery and has a constructor that takes JsExecutor.", e);
            }
        }
    }
} 