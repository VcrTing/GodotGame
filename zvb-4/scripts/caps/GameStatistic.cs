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
    public bool IsAllZombieDead()
    {
        bool a = ZombieDeadCount > 0 && ZombieDeadCount == ZombieChapterTotal;
        return a;
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
	}
}
