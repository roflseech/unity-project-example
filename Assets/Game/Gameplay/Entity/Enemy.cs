using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Gameplay.Entity
{
    public class Enemy : MonoBehaviour
    {
        [Header("Base components")]
        [SerializeField] private NavMeshAgent _agent;
        [Header("Base components")]
        [SerializeField] private List<Transform> _patrolPoints;
        [SerializeField] private float _patrolSpeed;
        [SerializeField] private float _chaseSpeed;

        private void Start()
        {
            //_agent.SetDestination()
        }
        
        
    }
}