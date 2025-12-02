using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject coin;
    public static int count;

    public bool is_boss;

    public float health;
    public float defense;
    public float speed;
    public int income;

    private void Awake()
    {
        count++;
        GetComponent<NavMeshAgent>().speed = speed;
    }
    private void Update()
    {
        if (health <= 0) Dead();
        if (transform.position.x <= 0.2) Goal();
    }

    private void Dead()
    {
        count--;
        for (int i = 0; i < income; i++)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void Goal()
    {
        if (is_boss) Game_Manager.Life = 0;
        else
        {
            Game_Manager.Life--;
            count--;
            Destroy(gameObject);
        }
    }
}
