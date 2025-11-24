using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class SpawnData
{
    public string name;
    public int delay;

    public SpawnData(string name, int delay)
    {
        this.name = name;
        this.delay = delay;
    }
}

public class Data_Round
{
    public List<SpawnData> data = new List<SpawnData>();
    public Data_Round(int round)
    {
        switch (round)
        {
            case 1:
                SetRound_1();
                break;
            case 2:
                SetRound_2();
                break;
<<<<<<< HEAD
=======
            case 3:
                SetRound_3();
                break;
            case 4:
                SetRound_4();
                break;
            case 5:
                SetRound_5();
                break;
>>>>>>> Test
            default:
                SetRound_1();
                break;
        }
    }

    private void SetRound_1()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 1));
    }
    private void SetRound_2()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 1));
    }
<<<<<<< HEAD
=======
    private void SetRound_3()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 1));
    }
    private void SetRound_4()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 1));
    }
    private void SetRound_5()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 1));
    }
>>>>>>> Test
}
