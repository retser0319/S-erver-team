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
        GetComponent<NavMeshAgent>().speed = speed;
    }
    void Update()
    {
        // 1) 골 도달 처리
        if (transform.position.x <= 0.2f)
        {
            Goal();
            return;
        }

        // 2) 체력이 0 이하면, 웨이브 주인만 서버에 죽음 알림
        if (health <= 0 && !deathRequested)
        {
            deathRequested = true;

            if (GameClient.Instance != null)
            {
                // 웨이브 주인만 ENEMY:DEAD 전송
                if (GameClient.LocalPlayerId == Round_Manager.waveOwnerSlot)
                {
                    Vector2 pos = transform.position;
                    GameClient.Instance.SendEnemyDead(income, pos);
                }
                // 실제 LocalDead() 호출은 ENEMY:DEAD 메시지에서 처리
            }
            else
            {
                // 싱글플레이 or 네트워크 없는 경우
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
