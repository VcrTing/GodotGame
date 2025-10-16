using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterShiLiu : Node2D, IObj, IPao
{
    
    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;
    [Export]
    public string ObjName { get; set; } = PlansConstants.PeaGold;
    public string GetObjName() => ObjName;
    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.PeaGold;
        LoadBullet();
        DoInitEffect(GlobalPosition);
        init = true;
        return true;
    }
    int deg = 12; // 8 + 4 + 1
    // 散射
    public List<Vector2> ComputedAttackDirectionsExtra(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        ds.Add(direction);
        ds.Add(GameTool.RotateVector2(direction, 2));
        ds.Add(GameTool.RotateVector2(direction, -2));
        // LLL: -6到6之间，每0.5度计算一个方向
        for (float d = -deg; d <= deg; d += 3f)
        {
            if (d >= -2 && d <= 2) continue;
            ds.Add(GameTool.RotateVector2(direction, d));
        }
        return ds;
    }
    // 自射
    public List<Vector2> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        ds.Add(direction);
        for (float d = -2; d <= 2; d += 2f)
        {
            ds.Add(GameTool.RotateVector2(direction, d));
        }
        return ds;
    }
    void __Shoot(PackedScene bulletScene, Vector2 startPosition, Vector2 direction)
    {
        var bullet = bulletScene.Instantiate<Node2D>();
        bullet.Position = startPosition;
        // 调整运动方向
        if (bullet is IBulletBase bulletBase)
        {
            bulletBase.SetDirection(direction.Normalized());
        }
        GetTree().CurrentScene.AddChild(bullet);
    }
    public async void ShootBullet(Vector2 startPosition, Vector2 direction)
    {
        // 正式攻击
        var bulletScene = GD.Load<PackedScene>(BulletScenePath);
        if (bulletScene != null)
        {
            List<Vector2> directions = ComputedAttackDirections(startPosition, direction);
            foreach (var dir in directions)
            {
                __Shoot(bulletScene, startPosition, dir);
            }
            // 播放音效
            DoFireEffect(startPosition);
            //
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
            directions = ComputedAttackDirectionsExtra(startPosition, direction);
            foreach (var dir in directions)
            {
                __Shoot(bulletScene, startPosition, dir);
            }
        }
    }
    // 获取射手对应的子弹实例
    [Export]
    public string BulletScenePath { get; set; } = string.Empty;
    public void LoadBullet()
    {
        BulletScenePath = BulletConstants.GetBullet(ObjName);
    }
    public void DoFireEffect(Vector2 position)
    {
        // 播放音效
        SoundPlayerController.Instance?.EnqueueSound("Bullets/" + ObjName, ObjName, 4);
    }
    public void DoInitEffect(Vector2 position)
    {
        // 播放音效
        SoundPlayerController.Instance.EnqueueSound("Fx/upshooter", "up", 4);
    }
    public void DoFireLoadEffect(Vector2 position)
    {
        SoundFxController.Instance.PlayFx("Bullets/" + ObjName + "/Load", ObjName + "_load", 4, position);
    }
    public bool Die()
    {
        QueueFree();
        return true;
    }

    public void DoRotingEffect(Vector2 direction)
    {
        throw new NotImplementedException();
    }
}
