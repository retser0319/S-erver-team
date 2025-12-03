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
    public void AddCoin(int p, int coin)
    {
        if (p == 1) P1_Coin += coin;
        else if (p == 2) P2_Coin += coin;
        else if (p == 3) P3_Coin += coin;
    }
    public void GiveCoin(int p)
    {
        if (GameClient.Instance != null && GameClient.LocalPlayerId > 0)
        {
            if (GetCoin(GameClient.LocalPlayerId) <= 0)
                return;

            GameClient.Instance.SendGiveCoin(p, 1);
            return;
        }
        ApplyGiveCoin(GameClient.LocalPlayerId, p, 1);
    }

    public int GetCoin(int p)
    {
        if (p == 1) return P1_Coin;
        if (p == 2) return P2_Coin;
        if (p == 3) return P3_Coin;
        return 0;
    }

    public void ApplyGiveCoin(int from, int to, int amount)
    {
        if (amount <= 0) return;

        if (from == 1 && P1_Coin >= amount) P1_Coin -= amount;
        else if (from == 2 && P2_Coin >= amount) P2_Coin -= amount;
        else if (from == 3 && P3_Coin >= amount) P3_Coin -= amount;
        else return;

        AddCoin(to, amount);
    }
    public void CloseTab()
    {
        tileManager.ResetSelectedTile();
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
