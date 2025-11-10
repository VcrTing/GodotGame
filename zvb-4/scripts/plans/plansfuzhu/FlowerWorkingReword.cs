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
    public float ChangeLevelTime = 999;
    public int RewordCountLevel1 = 1;
    public int RewordCountLevel2 = 1;
    public string RewordName = SunMoneyConstants.Sun;

    IObj iobj;
    string objName;

    int level = 1;

    public override void _Ready()
    {
        int a = GD.RandRange(0, 200);
        float v = a / 100;
        FirstGenerateInterval += v;
        level = 1;
        __t = 0.00001f;

        iobj = GetParent<IObj>();
        objName = iobj.GetObjName();
        FirstGenerateInterval = SunMoneyConstants.GetSunPlansFirstGenTime(objName);
        EveryGenerateInterval = SunMoneyConstants.GetSunPlansEveryGenTime(objName);
        RewordValueLevel1 = SunMoneyConstants.GetSunPlansSunLevel1(objName);
        RewordValueLevel2 = SunMoneyConstants.GetSunPlansSunLevel2(objName);
        RewordCountLevel1 = SunMoneyConstants.GetSunPlansSunCount1(objName);
        RewordCountLevel2 = SunMoneyConstants.GetSunPlansSunCount2(objName);
        ChangeLevelTime = SunMoneyConstants.GetSunPlansGrowTime(objName);
        RewordName = SunMoneyConstants.GetPlansRewordType(objName);
        /*
        GD.Print("objName =" + objName);
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
            FirstGenerateInterval = RebuildForCountGroup(RandomGenerateInterval());
            R();
            SoundFxController.Instance?.PlayFx("Plans/" + obj.GetObjName(), obj.GetObjName(), 4);
            sunTimer = 0f;
        }
    }

    void R()
    {
        if (level == 1)
        {
            GameTool.GenReword(RewordName, RewordCountLevel1, RewordValueLevel1, this);
        }
        else if (level == 2)
        {
            GameTool.GenReword(RewordName, RewordCountLevel2, RewordValueLevel2, this);
        }
    }
    
    public float RebuildForCountGroup(float src)
    {
        var cs = GameStatistic.Instance;
        if (cs != null)
        {
            int c = cs.GetPlansCount(objName);
            if (c > 4)
            {
                src += 0.5f;
            }
            if (c > 7)
            {
                src += 0.5f;
            }
            src += ((c - 3) * 0.3f);
        }
        return src;
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
