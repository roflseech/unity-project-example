using UnityEngine;

namespace Game.Gameplay.Character
{
    public class CharacterVisual : MonoBehaviour
    {
        [SerializeField] private Transform _characterTransform;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _mouseThreshold = 0.1f;
        
        private bool _isStanding = true;
        private Quaternion _standingRotation;
        private Vector3 _lastMouseLookPoint;
        private bool _hasMouseMoved;

        public void LookAt(Vector3 point)
        {
            if (_isStanding)
            {
                var mouseMovement = Vector3.Distance(point, _lastMouseLookPoint);
                if (mouseMovement > _mouseThreshold)
                {
                    _hasMouseMoved = true;
                    var directionToMouse = (point - transform.position).normalized;
                    var localMouseDir = Quaternion.Inverse(_standingRotation) * directionToMouse;
                    _animator.SetFloat("Velocity X", localMouseDir.x);
                }
                else if (!_hasMouseMoved)
                {
                    _animator.SetFloat("Velocity X", 0f);
                }
                _lastMouseLookPoint = point;
            }
            
            _characterTransform.localRotation = Quaternion.LookRotation(point - transform.position, Vector3.up);
        }

        public void SetMoveDir(Vector3 dir)
        {
            var isMoving = dir.sqrMagnitude > Mathf.Epsilon;
            
            if (isMoving && _isStanding)
            {
                _isStanding = false;
                _hasMouseMoved = false;
            }
            else if (!isMoving && !_isStanding)
            {
                _isStanding = true;
                _standingRotation = _characterTransform.localRotation;
                _lastMouseLookPoint = transform.position + _characterTransform.forward;
                _hasMouseMoved = false;
                _animator.SetFloat("Velocity X", 0f);
            }
            
            if (isMoving)
            {
                var localDir = _characterTransform.InverseTransformDirection(dir);
                _animator.SetFloat("Velocity X", localDir.x);
                _animator.SetFloat("Velocity Z", localDir.z);
            }
            else
            {
                _animator.SetFloat("Velocity Z", 0f);
            }
            
            _animator.SetBool("Moving", isMoving);
        }
    }
}