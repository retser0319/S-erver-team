using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    [SerializeField] private Tile_Manager tileManager;
    [SerializeField] private Round_Manager roundManager;
    [SerializeField] public GameObject[] Towers;
    [SerializeField] public TMP_Text UI_Life;
    [SerializeField] public TMP_Text UI_P1_Coin;
    [SerializeField] public TMP_Text UI_P2_Coin;
    [SerializeField] public TMP_Text UI_P3_Coin;

    public static int Life = 100;
    public static int P1_Coin = 10;
    public static int P2_Coin = 10;
    public static int P3_Coin = 10;


    private void Update()
    {
        UI_Renewal_Life();
        UI_Renewal_Coin();
    }
    public void UI_Renewal_Life()
    {
        UI_Life.text = Life.ToString();
    }
    public void UI_Renewal_Coin()
    {
        UI_P1_Coin.text = P1_Coin.ToString();
        UI_P2_Coin.text = P2_Coin.ToString();
        UI_P3_Coin.text = P3_Coin.ToString();
    }
    public void AddCoin(int coin)
    {
        if (GameClient.LocalPlayerId == 1) P1_Coin += coin;
        if (GameClient.LocalPlayerId == 2) P2_Coin += coin;
        if (GameClient.LocalPlayerId == 3) P3_Coin += coin;
    }

    public void CreateRandomTower()
    {
        if (GameClient.LocalPlayerId == 1 && P1_Coin >= 5) P1_Coin -= 5;
        else if (GameClient.LocalPlayerId == 2 && P2_Coin >= 5) P2_Coin -= 5;
        else if (GameClient.LocalPlayerId == 3 && P3_Coin >= 5) P3_Coin -= 5;
        else return;

        Transform wall = tileManager.selectedTile.transform;
        int num = Random.Range(0, Towers.Length);
        Instantiate(Towers[num], wall.position, Quaternion.identity, wall);
        tileManager.ResetSelectedTile();
    }
    public void RemoveTower()
    {
        if (GameClient.LocalPlayerId == 1) P1_Coin += 2;
        if (GameClient.LocalPlayerId == 2) P2_Coin += 2;
        if (GameClient.LocalPlayerId == 3) P3_Coin += 2;

        if (tileManager.selectedTile.transform.GetChild(0) != null)
            Destroy(tileManager.selectedTile.transform.GetChild(0).gameObject);
        tileManager.ResetSelectedTile();
    }
}
