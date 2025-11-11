using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class NavMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 20f;
    public NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.SetDestination(new Vector3(34, 2, -29));
    }

    private void Update()
    {
        if (Vector3.Distance(navMeshAgent.destination, transform.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}