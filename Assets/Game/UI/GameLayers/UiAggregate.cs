using System.Collections.Generic;
using Game.UI.Provider;

namespace Game.UI.GameLayers
{
    public interface IUiAggregate
    {
        IUiProvider Get(UiLayer layer);
    }
    
    public class UiAggregate : IUiAggregate
    {
        private readonly Dictionary<UiLayer, Entry> _entries = new();
        
        public IUiProvider Get(UiLayer layer)
        {
            return GetEntry(layer).Provider;
        }
        
        public void SetupLayer(IUiScene uiScene, IUiProvider uiProvider, UiLayer layer)
        {
            var entry = GetEntry(layer);
            
            entry.Provider = uiProvider;
            entry.Scene = uiScene;
        }
        
        private Entry GetEntry(UiLayer layer)
        {
            if (!_entries.TryGetValue(layer, out var entry))
            {
                entry = new Entry();
                _entries[layer] = entry;
            }
            
            return entry;
        }
        
        private class Entry
        {
            public IUiProvider Provider;
            public IUiScene Scene;
        }
    }
}