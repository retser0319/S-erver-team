using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public static int count = 0;
    public float health = 10;
    public float defense = 0;
    public float speed = 2;

    private void Awake()
    {
        count++;
        GetComponent<NavMeshAgent>().speed = speed;
    }
    private void Update()
    {
        if (health <= 0) Destroy(gameObject);
    }
    private void OnDestroy()
    {
        count--;
    }
}
