using System;
using System.Collections.Generic;
using Jint;
using Jint.Native;
using Jint.Runtime;

namespace Game.JsBridge
{
    public static class BaseJsConverters
    {
        public static int AsInt(this JsValue value)
        {
            return (int)value.AsNumber();
        }
        
        public static string AsString(this JsValue value)
        {
            return TypeConverter.ToString(value);
        }
        
        public static bool AsBoolean(this JsValue value)
        {
            return TypeConverter.ToBoolean(value);
        }
        
        public static float AsFloat(this JsValue value)
        {
            return (float)value.AsNumber();
        }
        
        public static double AsDouble(this JsValue value)
        {
            return value.AsNumber();
        }

        public static IReadOnlyList<T> AsList<T>(this JsValue value, Func<JsValue, T> converter)
        {
            var array = value.AsArray();
            
            var list = new List<T>();
            
            foreach (var element in array)
            {
                list.Add(converter(element));
            }

            return list;
        }
    }
}