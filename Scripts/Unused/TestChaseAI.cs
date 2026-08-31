using System;
using UnityEngine;
using UnityEngine.AI;

public class TestChaseAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning("TestChaseAI: No NavMeshAgent component attached");
            return;
        }
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void Update()
    {
        agent.SetDestination(target.position);
    }
}
