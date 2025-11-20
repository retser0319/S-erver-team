using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] GameObject Enemy;

    public bool round_in_progress = false;
    public void RoundStart()
    {
        round_in_progress = true;
        SpawnEnemy();
    }


    public void SpawnEnemy()
    {
        Instantiate(Enemy, new Vector2(34, 4), Quaternion.identity);
    }
}
