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

    public string GetRandomPlansNameWithPowerWeight()
    {
        string n = GetNameByWeight();
        GD.Print("加权随机到的植物是: " + n);
        if (n == "")
        {
            n = PlansConstants.GetRandomPlansName();
            GD.Print("无，随机到的植物是: " + n);
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

}
