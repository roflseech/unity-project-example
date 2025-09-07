using UnityEditor;
using UnityEngine;

namespace Game.EditorFlow
{
    [InitializeOnLoad]
    public static class PlayModeEnter
    {
        static PlayModeEnter()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                AssetDatabase.Refresh();
            }
        }
    }
} 