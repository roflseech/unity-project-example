using UnityEngine;

namespace Game.Gameplay.CameraManagement
{
    public interface ICameraProvider
    {
        void SetTopDownFollowTarget(GameObject target);
        Camera Camera { get; }
    }
}