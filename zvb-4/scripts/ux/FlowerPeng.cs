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
        Test();
    }

    public void Test()
    {
        // 加载场景
        var scene = GD.Load<PackedScene>("res://wavehouse/player/plans_base_miao.tscn");
        if (scene == null)
        {
            GD.PrintErr("加载 plans_base_miao.tscn 失败");
            return;
        }
        // 生成实例
        var instance = scene.Instantiate();
        AddChild(instance);
        // 获取PlansBaseMiao脚本
        if (instance is PlansBaseMiao miao)
        {
            // 随机获取PlanSceneDict的键
            var keys = ZVB4.Conf.PlansConstants.PlanSceneDict.Keys.ToList();
            var rand = new Random();
            string key = keys[rand.Next(keys.Count)];
            miao.Init(key);
        }
        else
        {
            GD.PrintErr("实例未绑定PlansBaseMiao脚本");
        }
    }
}
