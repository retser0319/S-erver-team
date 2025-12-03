using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public static int P1_Coin = 100;
    public static int P2_Coin = 100;
    public static int P3_Coin = 100;


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
    public void ApplyWallPlace(int p, int x, int y, int type)
    {
        if (GetCoin(p) < 2)
        {
            Debug.Log($"[GAME] P{p} 코인 부족. 벽 설치 실패.");
            return;
        }

        AddCoin(p, -2);
        tileManager.TileChange(x, y, type);
    }
    public void ApplyWallRemove(int p, int x, int y, int type)
    {
        AddCoin(p, 2);
        tileManager.TileChange(x, y, type);
    }
    public void CloseTab()
    {
        tileManager.ResetSelectedTile();
    }
    public void ApplyTowerPlace(int p, int x, int y, int towerIndex)
    {
        if (GetCoin(p) < 5)
        {
            Debug.Log($"[GM] ApplyTowerPlace: P{p} 코인 부족");
            return;
        }

        AddCoin(p, -5);

        if (x < 0 || x >= tileManager.xSize || y < 0 || y >= tileManager.ySize)
        {
            Debug.LogWarning($"[GM] ApplyTowerPlace: 잘못된 타일 좌표 ({x},{y})");
            return;
        }

        GameObject tile = tileManager.map[y, x];
        if (tile == null)
        {
            Debug.LogWarning($"[GM] ApplyTowerPlace: tileManager.map[{y},{x}] 가 null");
            return;
        }

        Transform t = tile.transform;

        // 기존 타워 있으면 전부 제거 (첫 자식만 써도 되지만 안전하게 전체 삭제)
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Destroy(t.GetChild(i).gameObject);
        }

        if (towerIndex < 0 || towerIndex >= Towers.Length)
        {
            Debug.LogWarning($"[GM] ApplyTowerPlace: 잘못된 towerIndex {towerIndex}");
            return;
        }

        // 타워 생성
        Instantiate(Towers[towerIndex], t.position, Quaternion.identity, t);
    }
    public void ApplyTowerRemove(int p, int x, int y)
    {
        AddCoin(p, 2);

        if (tileManager == null || tileManager.map == null)
            return;

        if (y < 0 || y >= tileManager.ySize || x < 0 || x >= tileManager.xSize)
            return;

        GameObject tile = tileManager.map[y, x];
        if (tile == null)
            return;

        Transform t = tile.transform;

        if (t.childCount > 0)
        {
            Destroy(t.GetChild(0).gameObject);
        }
        else
        {
            Debug.Log($"[GM] ApplyTowerRemove: 자식이 없어서 제거할 타워가 없음 ({x},{y})");
        }
    }
    public void CreateRandomTower()
    {
        if (tileManager.selectedTile == null)
        {
            Debug.LogWarning("CreateRandomTower: 선택된 타일이 없음");
            return;
        }

        Vector3 pos = tileManager.selectedTile.transform.position;
        int x = (int)pos.x;
        int y = (int)pos.y;

        int towerIndex = Random.Range(0, Towers.Length);

        if (GameClient.Instance != null && GameClient.LocalPlayerId > 0)
        {
            int p = GameClient.LocalPlayerId;

            if (GetCoin(p) < 5)
            {
                Debug.Log("[GM] 타워 설치 실패: 코인 부족");
                return;
            }

            GameClient.Instance.SendTowerPlace(p, x, y, towerIndex);
        }
        else
        {
            ApplyTowerPlace(1, x, y, towerIndex);
        }

        tileManager.ResetSelectedTile();
    }
    public void RemoveTower()
    {
        if (tileManager.selectedTile == null)
        {
            Debug.LogWarning("RemoveTower: selectedTile 이 null 입니다.");
            return;
        }

        Transform t = tileManager.selectedTile.transform;
        int x = (int)t.position.x;
        int y = (int)t.position.y;
        int p = GameClient.LocalPlayerId;

        if (GameClient.Instance != null && p > 0)
        {
            if (t.childCount > 0)
            {
                GameClient.Instance.SendTowerRemove(p, x, y);
            }
            else
            {
                GameClient.Instance.SendWallRemove(p, x, y);
            }
        }
        else
        {
            if (t.childCount > 0)
            {
                ApplyTowerRemove(p <= 0 ? 1 : p, x, y);
            }
            else
            {
                ApplyWallRemove(p <= 0 ? 1 : p, x, y, 0);
            }
        }
        tileManager.ResetSelectedTile();
    }
}
