using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class FlowerWorkingReword: Node2D
{
    public float sunTimer = 0f;
    [Export]
    public float FirstGenerateInterval = 3f; // 首次3秒
    [Export]
    public float EveryGenerateInterval = 10f; // 每次间隔

    public int GetRewordValue() => RewordValue;
    public int GetRewordCount() => RewordCount;
    public string GetRewordName() => RewordName;

    [Export]
    public int RewordValue = 50;
    [Export]
    public int RewordCount = 1;
    [Export]
    public string RewordName = SunMoneyConstants.Sun;


    public void ProcessingForFlower(double delta, IObj obj)
    {
        sunTimer += (float)delta;
        if (sunTimer >= FirstGenerateInterval)
        {
            FirstGenerateInterval = RandomGenerateInterval();
            GameTool.GenReword(RewordName, RewordCount, RewordValue, this);
            SoundFxController.Instance?.PlayFx("Plans/" + obj.GetObjName(), obj.GetObjName(), 4);
            sunTimer = 0f;
        }
    }

    public float RandomGenerateInterval()
    {
        var rand = new Random();
        float randomOffset = (float)(rand.NextDouble() * 4.0 - 2.0); // -2 到 +2 之间的随机数
        float newInterval = EveryGenerateInterval + randomOffset;
        if (newInterval < 5f) newInterval = 5f;
        if (newInterval > 15f) newInterval = 15f;
        return newInterval;
    }

}
