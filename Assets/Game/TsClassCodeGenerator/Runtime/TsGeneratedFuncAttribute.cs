using System;

namespace Game.TsClassCodeGenerator.Runtime
{
    /// <summary>
    /// Attribute that marks classes generated from TypeScript functions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TsGeneratedFuncAttribute : Attribute
    {
    }
} 