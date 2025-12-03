using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Round_Manager : MonoBehaviour
{
    [SerializeField] public GameObject[] enemy;
    [SerializeField] public TMP_Text UI_Round;

    public bool round_in_progress = false;
    public int round = 0;

    private float tick = 0;
    private int index = 0;
    private float delay = 1;

    private List<SpawnData> data = new List<SpawnData>();

    public static int waveOwnerSlot = -1;

    void Update()
    {
        UI_Round.text = round.ToString() + "|15";
        if (GameClient.Instance != null &&
            GameClient.LocalPlayerId > 0 &&
            waveOwnerSlot > 0 &&
            GameClient.LocalPlayerId != waveOwnerSlot)
        {
            return;
        }

        if (Enemy.count == 0 && data.Count == 0) round_in_progress = false;

        if (data.Count == 0) return;
        tick += Time.deltaTime;
        if (tick > delay)
        {
            tick = 0;
            if (index < data.Count)
            {
                SpawnEnemy(data[index].name);

                index++;
                if (index < data.Count)
                    delay = data[index].delay;
                else
                    delay = 0;
            }
            else
            {
                index = 0;
                data.Clear();
            }
        }
    }

    public void StartWaveFromNetwork(int ownerSlot)
    {
        waveOwnerSlot = ownerSlot;
        if (round_in_progress) return;
        RoundStart();
    }

    public void RoundStart()
    {
        if (IsPathBlocked(new Vector2(34, 6), new Vector2(0, 6)) || round_in_progress) return;
        round++;
        round_in_progress = true;
        data = new Data_Round(round).data;
        delay = data[0].delay;
    }

    public void SpawnEnemy(string name)
    {
        float y = Random.Range(0f, 8f);
        Vector2 pos = new Vector2(34, y);

        switch (name)
        {
            case "basic":
                Instantiate(enemy[0], pos, Quaternion.identity);
                break;
            case "speed":
                Instantiate(enemy[1], pos, Quaternion.identity);
                break;
            case "hardness":
                Instantiate(enemy[2], pos, Quaternion.identity);
                break;
            case "fly":
                Instantiate(enemy[3], pos, Quaternion.identity);
                break;
            case "bee":
                Instantiate(enemy[6], pos, Quaternion.identity);
                break;
            case "boss_1":
                Instantiate(enemy[4], new Vector2(34, 4), Quaternion.identity);
                break;
            case "boss_2":
                Instantiate(enemy[5], new Vector2(34, 4), Quaternion.identity);
                break;
        }

        if (GameClient.Instance != null &&
            GameClient.LocalPlayerId == waveOwnerSlot)
        {
            GameClient.Instance.SendEnemySpawn(name, pos);
        }
    }

    public void SpawnEnemyFromNetwork(string name, Vector2 pos)
    {
        switch (name)
        {
            case "basic":
                Instantiate(enemy[0], pos, Quaternion.identity);
                break;
            case "speed":
                Instantiate(enemy[1], pos, Quaternion.identity);
                break;
            case "hardness":
                Instantiate(enemy[2], pos, Quaternion.identity);
                break;
            case "fly":
                Instantiate(enemy[3], pos, Quaternion.identity);
                break;
            case "boss_1":
                Instantiate(enemy[4], pos, Quaternion.identity);
                break;
            case "boss_2":
                Instantiate(enemy[5], pos, Quaternion.identity);
                break;
            case "bee":
                Instantiate(enemy[6], pos, Quaternion.identity);
                break;
        }
    }
    public bool IsPathBlocked(Vector3 startPos, Vector3 endPos)
    {
        NavMeshPath path = new NavMeshPath();

        // NavMesh 경로 계산
        bool hasPath = NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path);

        // 경로가 없으면 이미 막힌 상태
        if (!hasPath)
            return true;

        // 경로가 있지만 실제로 길이 끊겨 있을 수 있으므로 상태 체크
        if (path.status != NavMeshPathStatus.PathComplete)
            return true;

        // 여기까지 왔으면 완전한 경로 존재 → 막히지 않음
        return false;
    }
}
