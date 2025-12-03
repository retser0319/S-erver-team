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
    private bool deathRequested = false;

    private void Awake()
    {
        count++;
        if (TryGetComponent(out NavMeshAgent agent)) agent.speed = speed;
        if (TryGetComponent(out AI_Fly fly)) fly.speed = speed;
    }
    void Update()
    {
        if (transform.position.x <= 0.2f)
        {
            Goal();
            return;
        }

        if (health <= 0 && !deathRequested)
        {
            deathRequested = true;

            if (GameClient.Instance != null)
            {
                if (GameClient.LocalPlayerId == Round_Manager.waveOwnerSlot)
                {
                    Vector2 pos = transform.position;
                    GameClient.Instance.SendEnemyDead(income, pos);
                }
            }
            else
            {
                LocalDead();
            }
        }
    }

    public void LocalDead()
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
