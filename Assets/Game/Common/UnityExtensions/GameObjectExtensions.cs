using UnityEngine;

namespace Game.Common.UnityExtensions
{
    public static class GameObjectExtensions
    {
        public static void ClearObjectsUnderTransform(this Transform transform)
        {
            foreach (Transform t in transform)
            {
                Debug.Log(t);
                if (t != transform) GameObject.Destroy(t.gameObject);
            }
        }
    }
}