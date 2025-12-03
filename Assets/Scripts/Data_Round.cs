using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class SpawnData
{
    public string name;
    public float delay;

    public SpawnData(string name, float delay)
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
            case 3:
                SetRound_3();
                break;
            case 4:
                SetRound_4();
                break;
            case 5:
                SetRound_5();
                break;
            case 6:
                SetRound_6();
                break;
            case 7:
                SetRound_7();
                break;
            case 8:
                SetRound_8();
                break;
            case 9:
                SetRound_9();
                break;
            case 10:
                SetRound_10();
                break;
            case 11:
                SetRound_11();
                break;
            case 12:
                SetRound_12();
                break;
            case 13:
                SetRound_13();
                break;
            case 14:
                SetRound_14();
                break;
            case 15:
                SetRound_15();
                break;    
            default:
                SetRound_1();
                break;
        }
    }
    // basic : 기본, speed : 빠른거, hardness : 단단한거, fly : 날아다니는거, boss_1 : 보스 1, boss_2 : 보스 2
    private void SetRound_1()
    {
        for (int i = 0; i < 10; i++)
        {
            data.Add(new SpawnData("basic", 0.5f));
        }
    }
    private void SetRound_2()
    {
        for (int i = 0; i < 10; i++){
            data.Add(new SpawnData("basic", 0.5f));
            data.Add(new SpawnData("speed", 0.5f));
            
        }
    }
    private void SetRound_3()
    {
        for (int i = 0; i < 15; i++){
            data.Add(new SpawnData("basic", 0.5f));
            data.Add(new SpawnData("speed", 0.5f));
            if (i % 3 == 0)
                data.Add(new SpawnData("hardness", 0.5f));}
    }
    private void SetRound_4()
    {
        for (int i = 0; i < 20; i++)
            data.Add(new SpawnData("basic", 0.2f));
    }
    private void SetRound_5()
    {
        for (int i = 0; i < 5; i++){
            data.Add(new SpawnData("speed", 0.1f));
        }
            data.Add(new SpawnData("boss_1", 1));
    }
    private void SetRound_6()
    {
        for (int i = 0; i < 5; i++)
            data.Add(new SpawnData("basic", 1));
        for ( int i = 0; i < 3; i++){
            data.Add(new SpawnData("hardness", 1));
            data.Add(new SpawnData("fly", 1));
        }
            
    }
    private void SetRound_7()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("speed", 0.3f));
    }
    private void SetRound_8()
    {
        for (int i = 0; i < 10; i++){
            data.Add(new SpawnData("hardness", 1));
            data.Add(new SpawnData("fly", 1));
        }
    }
    private void SetRound_9()
    {
        for (int i = 0; i < 5; i++){
            data.Add(new SpawnData("hardness", 1));
            data.Add(new SpawnData("speed", 0.5f));
        }
    }
    private void SetRound_10()
    {
        for(int j = 0;j < 10; j++)
        {
            data.Add(new SpawnData("bee", 0.5f));
        }
        for (int i = 0; i < 1; i++)
            data.Add(new SpawnData("boss_2", 1));   
    }
    private void SetRound_11()
    {
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("basic", 0.1f));
            data.Add(new SpawnData("bee", 0.1f));
        for (int i = 0; i < 5; i++){
            data.Add(new SpawnData("speed", 0.5f));
            data.Add(new SpawnData("fly", 0.5f));
            data.Add(new SpawnData("hardness", 0.5f));
        }
    }
    private void SetRound_12()
    {
        for (int i = 0; i < 15; i++)
            data.Add(new SpawnData("fly", 1));
    }
    private void SetRound_13()
    {
        for (int i = 0; i < 1; i++){
            data.Add(new SpawnData("boss_1", 5));
            data.Add(new SpawnData("boss_2", 5));
        }
        
    }
    private void SetRound_14()
    {
        for (int i = 0; i < 20; i++)
            data.Add(new SpawnData("basic", 1));
        for (int i = 0; i < 15; i++)
            data.Add(new SpawnData("speed", 1));
        for (int i = 0; i < 10; i++)
            data.Add(new SpawnData("fly", 1));
        for (int i = 0; i < 5; i++)
            data.Add(new SpawnData("hardness", 1));
    }
    private void SetRound_15()
    {
        for (int i = 0; i < 3; i++){
            data.Add(new SpawnData("boss_1", 5));
            data.Add(new SpawnData("boss_2", 5));
        }
    }
}
