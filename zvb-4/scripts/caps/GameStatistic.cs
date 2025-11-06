using Godot;
using System;

public partial class GameStatistic : Node2D
{
    public static GameStatistic Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        ResetAll();
    }
    // 僵尸数量
    public int ZombieCount { get; private set; } = 0;
    // 僵尸总数
    public int ZombieChapterTotal { get; private set; } = 0;
    // 僵尸死亡数量
    public int ZombieDeadCount { get; private set; } = 0;
    // 阳光产量
    public int SunProduced { get; private set; } = 0;
    // 阳光消耗数量
    public int SunConsumed { get; private set; } = 0;
    // 金币收集数量
    public int CoinCollected { get; private set; } = 0;
    //
    public void AddZombie(int count = 1)
    {
        ZombieCount += count;
    }
    public void SetZombieChapterTotal(int count = 1)
    {
        ZombieChapterTotal = count;
    }
    public void AddZombieDead(int count = 1)
    {
        ZombieDeadCount += count;
    }
	public void AddSunProduced(int count = 1) => SunProduced += count;
	public void AddSunConsumed(int count = 1) => SunConsumed += count;
	public void AddCoinCollected(int count = 1) => CoinCollected += count;

    public void ResetAll()
    {
        ZombieCount = 0;
        ZombieDeadCount = 0;
        SunProduced = 0;
        SunConsumed = 0;
        CoinCollected = 0;
        SunFlowerCount = 0;

        GuanZiChapterTotal = 0;
        ZombieChapterTotal = 0;

        GuanZiDieCount = 0;
    }
    // 本关向日葵数量
    public int SunFlowerCount { get; set; } = 0;
    // 植物名称-数量字典
    public System.Collections.Generic.Dictionary<string, int> PlantCountDict { get; private set; } = new System.Collections.Generic.Dictionary<string, int>();
    // 增加指定植物的数量（默认向日葵）
    public void AddPlansCount( string plantName, int count = 1)
    {
        if (!PlantCountDict.ContainsKey(plantName))
            PlantCountDict[plantName] = 0;
        PlantCountDict[plantName] += count;
    }
    public int GetPlansCount(string plantName)
    {
        if (PlantCountDict.ContainsKey(plantName))
            return PlantCountDict[plantName];
        return 0;
    }

    //
    
    // 罐子数量
    public int GuanZiChapterTotal { get; private set; } = 0;
    // 罐子打破数量
    public int GuanZiDieCount { get; private set; } = 0;

    //
    public bool WinCheckWhenChapterAllEnmyDie()
    {
        if (ZombieChapterTotal > 0)
        {
            return ZombieDeadCount > 0 && ZombieDeadCount >= ZombieChapterTotal;
        }
        return false;
    }
    public void AddGuanZiChapterTotal(int n = 1)
    {
        GuanZiChapterTotal += 1;
    }
    public void AddGuanziDie(int num = 1) {
        GuanZiDieCount += num;
    }
    public bool WinCheckWhenGuanZiMode()
    {
        bool star = false;
        if (GuanZiChapterTotal > 0)
        {
            star = GuanZiDieCount > 0 && GuanZiDieCount == GuanZiChapterTotal;
            // GD.Print("检查罐子star = " + GuanZiDieCount + " " + GuanZiChapterTotal + " true = " + ( ZombieDeadCount == ZombieCount));
            if (star) {
                return ZombieDeadCount == ZombieCount;
            }
        }
        return false;
    }
}
