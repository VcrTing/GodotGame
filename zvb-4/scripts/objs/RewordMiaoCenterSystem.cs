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
        String n = PlansConstants.GetRandomPlansName();
        AddOne(n);
        return n;
    }

    void ReComputedWeight()
    {
        foreach (var key in PlansRewordWeightDict.Keys)
        {
            int baseWeight = PlansRewordWeightDict[key];
            int subWeight = 0;
            if (PlansDict.ContainsKey(key))
            {
                PlansRewordWeightSubDict.TryGetValue(key, out subWeight);
            }
            int newWeight = baseWeight - subWeight;
            int lowest = 0;
            PlansRewordWeightLowestDict.TryGetValue(key, out lowest);
            if (newWeight < lowest)
            {
                newWeight = lowest;
            }
            PlansRewordWeightDict[key] = newWeight;
        }
    }

    public static readonly Dictionary<string, int> PlansRewordWeightDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 10 },
        { PlansConstants.XiguaBing, 5 },
        { PlansConstants.SunFlower, 30 },
        { PlansConstants.Cherry, 10 },
    };
    public static readonly Dictionary<string, int> PlansRewordWeightSubDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 2 },
        { PlansConstants.XiguaBing, 2 },
        { PlansConstants.SunFlower, 1 },
        { PlansConstants.Cherry, 0 },
    };
    public static readonly Dictionary<string, int> PlansRewordWeightLowestDict = new Dictionary<string, int>
    {
        { PlansConstants.Pea, 2 },
        { PlansConstants.XiguaBing, 2 },
        { PlansConstants.SunFlower, 10 },
        { PlansConstants.Cherry, 10 },
    };

    string GetNameByWeight()
    {
        // 加权随机
        int totalWeight = 0;
        foreach (var kv in PlansRewordWeightDict)
        {
            totalWeight += kv.Value;
        }
        if (totalWeight <= 0) return "";
        int rand = (int)GD.RandRange(0, totalWeight);
        int sum = 0;
        foreach (var kv in PlansRewordWeightDict)
        {
            sum += kv.Value;
            if (rand < sum)
            {
                AddOne(kv.Key);
                return kv.Key;
            }
        }
        return "";
    }
    bool usePower = false;
    public string GetRandomPlansNameWithPowerWeight()
    {
        string n = GetNameByWeight();
        if (!usePower) { n = ""; }
        // GD.Print("加权随机到的植物是: " + n);
        if (n == "")
        {
            n = PlansConstants.GetRandomPlansName();
            // GD.Print("无，随机到的植物是: " + n);
        }
        else
        {
            ReComputedWeight();
        }
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

        int v = (int)GD.RandRange(1, w);
        int v2 = (int)GD.RandRange(1, w);

        x += v;
        y += v2;
        DumpPlansMiao(new Vector2(x, y), name, playSound);
    }

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
        int penNum = FlowerPengSystem.Instance?.GetUseFullFlowerPeng().Count ?? 0;
        GD.Print("当前可用花盆数量: " + penNum);
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
    void DumpAllPlansMiao(Godot.Collections.Array initmiaolist)
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
