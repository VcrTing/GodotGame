using System.Linq;

using Godot;
using System;
using ZVB4.Conf;

public partial class FlowerPeng : Node2D
{

    [Export]
    public int code = 1;

    public override void _Ready()
    {
        // Test();
    }

    string MiaoScenePath = "plans_base_miao.tscn";

    public void GeneratePlansMiao(string plansName)
    {
        _PlansMiao(plansName);
        isLock = true;
    }
    public async void DelayGeneratePlansMiao(float delayTime, string plansName)
    {
        if (isLock) return;
        isLock = true;
        await this.ToSignal(this.GetTree().CreateTimer(delayTime + 0.1f), "timeout");
        _PlansMiao(plansName);
    }

    bool isLock = false;
    public void _PlansMiao(string plansName)
    {
        // 加载场景
        var scene = GD.Load<PackedScene>(FolderConstants.WavePlayer + MiaoScenePath);
        // 生成实例
        var instance = scene.Instantiate();
        AddChild(instance);
        if (instance is PlansBaseMiao miao)
        {
            miao.Init(plansName);
        }
    }

    public bool IsAllowPlans()
    {
        return !isLock;
    }
    public void ReleaseLock()
    {
        isLock = false;
    }

    public void Test()
    {
        GeneratePlansMiao(PlansConstants.GetRandomPlansName());
    }
}
