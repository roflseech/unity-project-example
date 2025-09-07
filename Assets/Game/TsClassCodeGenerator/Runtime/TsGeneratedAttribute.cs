using System;

namespace Game.TsClassCodeGenerator.Runtime
{
    /// <summary>
    /// This attribute marks C# classes generated from TypeScript.
    /// Any class marked with this attribute will be removed by code generator if ts version is missing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
    public class TsGeneratedAttribute : Attribute
    {
    }
}