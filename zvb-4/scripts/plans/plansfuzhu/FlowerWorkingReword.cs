using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class FlowerWorkingReword: Node2D
{
    public float FirstGenerateInterval = 2f; // 首次3秒
    public float EveryGenerateInterval = 8f; // 每次间隔

    public int GetRewordValue() => RewordValueLevel1;
    public int GetRewordCount() => RewordCountLevel1;
    public string GetRewordName() => RewordName;

    public int RewordValueLevel1 = 50;
    public int RewordValueLevel2 = 50;
    public float ChangeLevelTime = PlansConstants.SunPlansGrowTime;
    public int RewordCountLevel1 = 1;
    public int RewordCountLevel2 = 1;
    public string RewordName = SunMoneyConstants.Sun;

    IObj iobj;

    int level = 1;

    public override void _Ready()
    {
        int a = GD.RandRange(0, 200);
        float v = a / 100;
        FirstGenerateInterval += v;
        level = 1;
        __t = 0.00001f;

        iobj = GetParent<IObj>();
        FirstGenerateInterval = SunMoneyConstants.GetSunPlansFirstGenTime(iobj.GetObjName());
        EveryGenerateInterval = SunMoneyConstants.GetSunPlansEveryGenTime(iobj.GetObjName());
        RewordValueLevel1 = SunMoneyConstants.GetSunPlansSunLevel1(iobj.GetObjName());
        RewordValueLevel2 = SunMoneyConstants.GetSunPlansSunLevel2(iobj.GetObjName());
        RewordCountLevel1 = SunMoneyConstants.GetSunPlansSunCount1(iobj.GetObjName());
        RewordCountLevel2 = SunMoneyConstants.GetSunPlansSunCount2(iobj.GetObjName());
        ChangeLevelTime = SunMoneyConstants.GetSunPlansGrowTime(iobj.GetObjName());
        RewordName = SunMoneyConstants.GetPlansRewordType(iobj.GetObjName());
        /*
        GD.Print("iobj.GetObjName() =" + iobj.GetObjName());
        GD.Print("RewordName =" + RewordName);
        GD.Print("RewordValueLevel1 =" + RewordValueLevel1);
        GD.Print("EveryGenerateInterval =" + EveryGenerateInterval);
        */
    }

    float __t = 0.00001f;
    public override void _Process(double delta)
    {
        if (__t > 0f) {
            __t += (float)delta;
            if (__t > ChangeLevelTime) {
                level = 2;
                __t = 0f;
            }
        }
    }

    public float sunTimer = 0f;
    public void ProcessingForFlower(double delta, IObj obj)
    {
        sunTimer += (float)delta;
        if (sunTimer >= FirstGenerateInterval)
        {
            FirstGenerateInterval = RandomGenerateInterval();
            R();
            SoundFxController.Instance?.PlayFx("Plans/" + obj.GetObjName(), obj.GetObjName(), 4);
            sunTimer = 0f;
        }
    }

    void R() {
        if (level == 1)
        {
            GameTool.GenReword(RewordName, RewordCountLevel1, RewordValueLevel1, this);
        }
        else if (level == 2)
        {
            GameTool.GenReword(RewordName, RewordCountLevel2, RewordValueLevel2, this);
        }
    }

    public float RandomGenerateInterval()
    {
        int a = GD.RandRange(0, 200);
        float v = a / 100;
        // float randomOffset = (float)(rand.NextDouble() * 4.0 - 2.0); // -2 到 +2 之间的随机数
        float newInterval = EveryGenerateInterval + v;
        if (newInterval < 5f) newInterval = 5f;
        if (newInterval > 12f) newInterval = 12f;
        return newInterval;
    }

}
