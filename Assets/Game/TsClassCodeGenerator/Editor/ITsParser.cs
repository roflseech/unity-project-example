using System.Collections.Generic;

namespace Game.TsClassCodeGenerator.Editor
{
    public interface ITsParser
    {
        IReadOnlyDictionary<string, ParsedInterface> Interfaces { get; }
        IReadOnlyDictionary<string, ParsedFunction> Functions { get; }
        void Parse(string tsPath);
        bool IsPrimitive(string typeName);
    }
}