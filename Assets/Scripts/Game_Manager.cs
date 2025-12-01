using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] private Round_Manager roundManager;
    [SerializeField] public GameObject[] Towers;

    public int P1_Coin;
    public int P2_Coin;
    public int P3_Coin;

    public void AddCoin(int P, int coin)
    {
        if (P == 1) P1_Coin += coin;
        if (P == 2) P2_Coin += coin;
        if (P == 3) P3_Coin += coin;
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
