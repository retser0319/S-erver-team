using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    public static int count = 0;
    public float health = 10;
    public float defense = 0;
    public float speed = 2;
    public int income;

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
        for (int i = 0; i < income; i++)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
        }
        count--;
    }
}
