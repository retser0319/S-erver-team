using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] public GameObject[] Towers;
    [SerializeField] public GameObject Enemy;

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

    public void CreateRandomTower()
    {
        Transform wall = tileManager.selectedTile.transform;
        int num = Random.Range(0, Towers.Length);
        Instantiate(Towers[num], wall.position, Quaternion.identity, wall);
        tileManager.ResetSelectedTile();
    }
    public void RemoveTower()
    {
        if (tileManager.selectedTile.transform.GetChild(0) != null)
            Destroy(tileManager.selectedTile.transform.GetChild(0).gameObject);
        tileManager.ResetSelectedTile();
    }
}
