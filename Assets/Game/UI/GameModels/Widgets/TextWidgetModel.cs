using System;
using Unity.Collections;

namespace Game.UI.GameModels.Widgets
{
    public interface ITextWidgetModel
    {
        string Text { get; }
        bool IsLocalized { get; }
        string[] Arguments { get; }
    }
    
    public class TextWidgetModel : ITextWidgetModel
    {
        public string Text { get; }
        public bool IsLocalized { get; }
        public string[] Arguments { get; }

        public TextWidgetModel(string text, bool isLocalized)
        {
            Text = text;
            IsLocalized = isLocalized;
            Arguments = Array.Empty<string>();
        }
        
        public TextWidgetModel(string text, bool isLocalized, params string[] arguments)
        {
            Text = text;
            IsLocalized = isLocalized;
            Arguments = arguments;
        }
    }
}