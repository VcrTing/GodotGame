using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class RewordMiaoCenterSystem : Node2D
{
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
    public static readonly Dictionary<string, int> PlansStarPowerDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 0 },
        { PlansConstants.XiguaBing, 0 },
        { PlansConstants.SunFlower, 3 },
        { PlansConstants.Cherry, 3 },
    };
    public static readonly Dictionary<string, int> PlansPowerDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 0 },
        { PlansConstants.XiguaBing, 0 },
        { PlansConstants.SunFlower, 30 },
        { PlansConstants.Cherry, 50 },
    };
    bool RandomPlansForPower(string name)
    {
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
    // 随机地点出生苗
    public void DumpPlansMiaoRandomPosition(string name, bool playSound = true)
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
        DumpPlansMiao(new Vector2(x, y), name, playSound);
    }
    int count = 0;
    public void DumpPlansMiao(Vector2 pos, string name, bool playSound = true)
    {
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
        }
        catch (Exception e)
        {
            GD.PrintErr("生成苗失败: " + e.Message);
            return;
        }
    }
    public void DumpInitPlansMiao(string initmiaomode, string initmiaorandomnummode, Godot.Collections.Array initmiaolist)
    {
        switch (initmiaomode)
        {
            case "random":
                DumpRandomPlansMiao(initmiaorandomnummode, initmiaolist);
                break;
            case "all":
                DumpAllPlansMiao(initmiaolist);
                break;
            case "无":
                GD.Print("不生成苗");
                break;
        }
    }
    void DumpRandomPlansMiao(string initmiaorandomnummode, Godot.Collections.Array initmiaolist)
    {
        int num = 0;
        // int penNum = FlowerPengSystem.Instance?.GetUseFullFlowerPeng().Count ?? 0;
        switch (initmiaorandomnummode)
        {
            case "FlowerPeng":
                num = 6; // FlowerPengSystem.Instance?.GetUseFullFlowerPeng().Count ?? 0;
                break;
            default:
                num = 3;
                break;
        }
        for (int i = 0; i < num; i++)
        {
            string name = GetRandomPlansNameWithPowerWeight();
            if (name != null && name != "")
            {
                DumpPlansMiaoRandomPosition(name, false);
            }
        }
    }
    public void DumpAllPlansMiao(Godot.Collections.Array initmiaolist)
    {
        foreach (var item in initmiaolist)
        {
            string name = item.AsString();
            if (name != null && name != "")
            {
                DumpPlansMiaoRandomPosition(name, false);
            }
        }
    }
}
