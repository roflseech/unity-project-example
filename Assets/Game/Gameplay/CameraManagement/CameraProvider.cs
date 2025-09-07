using Unity.Cinemachine;
using UnityEngine;

namespace Game.Gameplay.CameraManagement
{
    public class CameraProvider : MonoBehaviour, ICameraProvider
    {
        [SerializeField] private CinemachineCamera _cinemachineCamera;
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
        
        public void SetTopDownFollowTarget(GameObject target)
        {
            _cinemachineCamera.Follow = target.transform;
        }
    }
}