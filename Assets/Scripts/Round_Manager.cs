using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Round_Manager : MonoBehaviour
{
    [SerializeField] public GameObject enemy;

    public bool round_in_progress = false;
    public int round = 0;

    private float tick = 0;
    private int index = 0;
    private int delay = 1;

    private List<SpawnData> data = new List<SpawnData>();
    
    // Update is called once per frame
    void Update()
    {
        if (!round_in_progress) return;
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
                round_in_progress = false;
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
                Instantiate(enemy, new Vector2(34, y), Quaternion.identity);
                break;
        }
    }
}
