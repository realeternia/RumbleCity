using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public static PlayerManager Instance{ private set; get; }

    private List<PlayerData> playerList = new List<PlayerData>();

    public int playerCount = 3;

    private int turnIndex;

    static PlayerManager()
    {
        Instance = new PlayerManager();
        Instance.Init();
    }

    public void Init()
    {
        playerList.Add(new PlayerData { Name = "旺仔", IsAI = false, Color = Color.blue });
        playerList.Add(new PlayerData { Name = "甲鱼", IsAI = true, Color = Color.red, GreedRate = 85 });
        playerList.Add(new PlayerData { Name = "八戒", IsAI = true, Color = Color.green, GreedRate = 40 });
    }

    public bool IsAllFinish()
    {
        foreach(var p in playerList)
        {
            if(p.Sword <= 0)
            {
                return false;
            }
        }
        return true;
    }

    public int GetTurn()
    {
        return turnIndex % playerCount;
    }

    public PlayerData GetPlayerData(int id)
    {
        return playerList[id - 1];
    }

    public void NextTurn()
    {
        turnIndex++;
    }
}
