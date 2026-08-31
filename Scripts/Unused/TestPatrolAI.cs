using System;
using UnityEngine;
using UnityEngine.AI;
public class TestPatrolAI : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    private int currentIndex;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (waypoints.Length <= 0 || agent == null)
        {
            Debug.LogWarning("TestPatrolAI: No NavMeshAgent component or waypoints attached");
            return;
        }
        agent.destination = waypoints[currentIndex].position;
    }
    private void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentIndex =  (currentIndex + 1) % waypoints.Length;
            agent.destination = waypoints[currentIndex].position;
        }
    }
}
