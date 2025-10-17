using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class RewordMiaoCenterSystem : Node2D
{
    public float GenerateRatio = 1f;
    public void SetGenerateRatio(float ratio)
    {
        if (ratio < 0f) ratio = 0f;
        if (ratio > 1f) ratio = 1f;
        GenerateRatio = ratio;
    }
    public static RewordMiaoCenterSystem Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }
    Godot.Collections.Array AllowMiaoList = PlansConstants.GetAllPlanNamesArray();
    public void SetAllowMiaoList(Godot.Collections.Array list)
    {
        if (list != null && list.Count > 0)
        {
            AllowMiaoList = list;
        }
    }
    Dictionary<string, int> PlansDict = new Dictionary<string, int>();

    float spawnTimer = 0f;
    float spawnInterval = 1f;
    public override void _Process(double delta)
    {
        spawnTimer += (float)delta;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            // SpawnRewordPlansMiao(3);
        }
    }
    void SpawnRewordPlansMiao(int count)
    {
        var scene = GD.Load<PackedScene>(FolderConstants.WaveObj + "reword_plans_miao.tscn");
        for (int i = 0; i < count; i++)
        {
            var instance = scene.Instantiate<Node2D>();
            AddChild(instance);
        }
    }
    public string GetRandomPlansName()
    {
        String n = PlansConstants.GetRandomPlansName(AllowMiaoList);
        AddOne(n);
        return n;
    }
    bool usePower = false;
    // 大于多少，开始加权。
    public static readonly Dictionary<string, int> PlansStarPowerDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 0 },
        { PlansConstants.XiguaBing, 0 },
        { PlansConstants.SunFlower, 3 },
        { PlansConstants.Cherry, 5 },
    };
    // 小于加权，才能生成
    public static readonly Dictionary<string, int> PlansPowerDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 0 },
        { PlansConstants.XiguaBing, 0 },
        { PlansConstants.SunFlower, 30 },
        { PlansConstants.Cherry, 70 },
    };
    bool RandomPlansForPower(string name)
    {
        // 先找花盆数量
        var ps = FlowerPengSystem.Instance?.GetUseFullFlowerPeng().Count ?? 0;
        var psall = FlowerPengSystem.Instance?.GetNowFlowerPengNum() ?? 0;
        if (ps >= (GameContants.FlowerPengNum)) return true;
        // 现在4个花盆以上，而且可用花盆只有2个了
        if (psall >= 4) {
            int n = GD.RandRange(0, 100);
            // GD.Print("当前占用花盆数量: " + ps + "，触发概率限制，随机值: " + n);
            if (ps <= 2) { if (n < 30) return false; }
            if (ps <= 0) { if (n < 50) return false; }
        }

        //
        int i = GD.RandRange(0, 100);
        int power = 0;
        int younum = GameStatistic.Instance?.GetPlansCount(name) ?? 0;
        int numlimit = PlansStarPowerDict.ContainsKey(name) ? PlansStarPowerDict[name] : 0;
        if (numlimit > 0)
        {
            if (younum >= numlimit)
            {
                power = PlansPowerDict.ContainsKey(name) ? PlansPowerDict[name] : 0;
                GD.Print("当前拥有 " + name + " 数量: " + younum + "，超过上限 " + numlimit + "，触发概率限制，概率值: " + power + " i = " + i);
            }
        }
        if (power <= 0) return true;
        return i < power;
    }
    public string GetRandomPlansNameWithPowerWeight()
    {
        string n = PlansConstants.GetRandomPlansName(AllowMiaoList);
        bool isAllow = RandomPlansForPower(n);
        if (!isAllow) return "";
        AddOne(n);
        return n;
    }
    void AddOne(string name)
    {
        if (PlansDict.ContainsKey(name))
        {
            PlansDict[name] += 1;
        }
        else
        {
            PlansDict[name] = 1;
        }
    }
    bool HasOne(string name)
    {
        return PlansDict.ContainsKey(name) && PlansDict[name] > 0;
    }
    /*

    */
    float w = GameContants.ScreenHalfW - 90;
    int count = 0;
    public void DumpPlansMiaoMust(Vector2 pos, string name, bool playSound = true) {
        try
        {
            var scene = GD.Load<PackedScene>(FolderConstants.WaveObj + "reword_plans_miao.tscn");
            var instance = scene.Instantiate<RewordPlansMiao>();
            instance.Init(pos, name);
            if (playSound)
            {
                SoundFxController.Instance?.PlayFx("Ux/suprise", "suprise", 4, GlobalPosition);
            }
            count += 1;
            string n = "Miao" + count;
            instance.Name = n;
            AddChild(instance);
            
            // 加一计数
            var gs = GameStatistic.Instance;
            if (gs != null) { gs.AddPlansCount(name, 1); }

        }
        catch (Exception e)
        {
            GD.PrintErr("生成苗失败: " + e.Message);
            return;
        }
    }
    public void DumpPlansMiao(Vector2 pos, string name, bool playSound = true)
    {
        int i = GD.RandRange(0, 100);
        // 倍率锁死
        if (i >= GenerateRatio * 100f) return;
        DumpPlansMiaoMust(pos, name, playSound);
    }
    // 随机地点出生苗
    public void DumpPlansMiaoRandomPosition(string name, bool playSound = true, bool must = false)
    {
        Vector2 pos = this.GlobalPosition;
        float x = pos.X;
        float y = pos.Y;
        //
        int v = (int)GD.RandRange(1, w);
        int v2 = (int)GD.RandRange(1, w);
        //
        x += v;
        y += v2;
        if (must) {
            DumpPlansMiao(new Vector2(x, y), name, playSound);
            return;
        }
        DumpPlansMiao(new Vector2(x, y), name, playSound);
    }
    public void DumpInitPlansMiao(string initmiaomode, string initmiaorandomnummode, Godot.Collections.Array initmiaolist)
    {

        DumpAllPlansMiao(initmiaolist);
        /*
        switch (initmiaomode)
        {
            case "random":
                DumpListRandomPlansMiao(initmiaorandomnummode, initmiaolist);
                break;
            case "all":
                DumpAllPlansMiao(initmiaolist);
                break;
            case "无":
                GD.Print("不生成苗");
                break;
        }
        */
    }
    /*
    void DumpListRandomPlansMiao(string initmiaorandomnummode, Godot.Collections.Array initmiaolist)
    {
        int num = 0;
        switch (initmiaorandomnummode)
        {
            case "FlowerPeng":
                num = FlowerPengSystem.Instance?.GetNowFlowerPengNum() ?? 6;
                break;
            default:
                num = 3;
                break;
        }
        DumpAllPlansMiao(initmiaolist);
    }
    */
    void DumpAllPlansMiao(Godot.Collections.Array initmiaolist)
    {
        foreach (var item in initmiaolist)
        {
            string name = item.AsString();
            if (name != null && name != "")
            {
                DumpPlansMiaoRandomPosition(name, false, true);
            }
        }
    }
}
