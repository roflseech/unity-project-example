using UnityEngine;

namespace Game.Gameplay.ContentProvider
{
    [CreateAssetMenu(fileName = "EntityConfig", menuName = "Gameplay/EntityConfig")]
    public class EntityConfig : ScriptableObject
    {
        [field: SerializeField] public string PlayerAsset { get; private set; }
    }
}