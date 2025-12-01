using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] private Round_Manager roundManager;
    [SerializeField] public GameObject[] Towers;
    [SerializeField] public TMP_Text UI_P1_Coin;
    [SerializeField] public TMP_Text UI_P2_Coin;
    [SerializeField] public TMP_Text UI_P3_Coin;

    public int P1_Coin;
    public int P2_Coin;
    public int P3_Coin;

    public void UI_Renewal_Coin()
    {
        UI_P1_Coin.text = P1_Coin.ToString();
        UI_P2_Coin.text = P2_Coin.ToString();
        UI_P3_Coin.text = P3_Coin.ToString();
    }
    public void AddCoin(int P, int coin)
    {
        if (P == 1) P1_Coin += coin;
        if (P == 2) P2_Coin += coin;
        if (P == 3) P3_Coin += coin;
        UI_Renewal_Coin();
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
