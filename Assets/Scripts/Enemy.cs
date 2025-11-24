using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float health = 10;
    public float defense = 0;
    public float speed = 2;

    private void Start()
    {
        GetComponent<NavMeshAgent>().speed = speed;
    }
    private void Update()
    {
        if (health <= 0) Destroy(gameObject);
    }
}
