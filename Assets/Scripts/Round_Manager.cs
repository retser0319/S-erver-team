using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Round_Manager : MonoBehaviour
{
    [SerializeField] public GameObject[] enemy;

    public bool round_in_progress = false;
    public int round = 0;

    private float tick = 0;
    private int index = 0;
    private float delay = 1;

    private List<SpawnData> data = new List<SpawnData>();
    
    void Update()
    {
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

    public void RoundStart()
    {
        if (round_in_progress) return;
        round++;
        round_in_progress = true;
        data = new Data_Round(round).data;
        delay = data[0].delay;
    }
    public void SpawnEnemy(string name)
    {
        float y = Random.Range(0f, 8f);
        switch (name)
        {
            case "basic":
                Instantiate(enemy[0], new Vector2(34, y), Quaternion.identity);
                break;
            case "speed":
                Instantiate(enemy[1], new Vector2(34, y), Quaternion.identity);
                break;
            case "hardness":
                Instantiate(enemy[2], new Vector2(34, y), Quaternion.identity);
                break;
            case "fly":
                Instantiate(enemy[3], new Vector2(34, y), Quaternion.identity);
                break;
            case "boss_1":
                Instantiate(enemy[4], new Vector2(34, y), Quaternion.identity);
                break;
            case "boss_2":
                Instantiate(enemy[5], new Vector2(34, y), Quaternion.identity);
                break;
        }
    }
}
