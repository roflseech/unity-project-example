using UnityEngine;

namespace Game.Configs.UI
{
    [CreateAssetMenu(fileName = "UiConfig", menuName = "Config/UiWindowsPaths")]
    public class UiWindowsPaths : ScriptableObject
    {
        [field: SerializeField] public string HomeScreenPath { get; private set; }
        [field: SerializeField] public string BaseGameplayWindowPath { get; private set; }
        [field: SerializeField] public string WinWindowPath { get; private set; }
    }
}