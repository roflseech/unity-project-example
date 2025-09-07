using System;
using Game.Gameplay.Character;
using UnityEngine;

namespace Game.Gameplay.Entity
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private CharacterVisual _characterVisual;
        [SerializeField] private GameObject _cloudReveal;
        
        private Vector3 _moveDirection;
        
        public GameObject GameObject => gameObject;
        public void Move(Vector2 dir)
        {
            if (dir.sqrMagnitude <= Mathf.Epsilon)
            {
                _moveDirection = Vector3.zero;
            }
            else
            {
                _moveDirection = new Vector3(dir.x, 0, dir.y).normalized;
            }

            _characterVisual.SetMoveDir(_moveDirection);
        }

        public void LookAt(Vector3 point)
        {
            _characterVisual.LookAt(point);
            var dir = point - transform.position;
            
            if (dir.sqrMagnitude > Mathf.Epsilon)
            {
                _cloudReveal.transform.localRotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveDirection * 3;
        }
    }
}