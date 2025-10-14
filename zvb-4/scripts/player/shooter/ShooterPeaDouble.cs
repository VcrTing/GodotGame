using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterPeaDouble : Node2D, IObj, IShooter
{
    
    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;

    [Export]
    public string ObjName { get; set; } = PlansConstants.LanMei;
    public string GetObjName() => ObjName;

    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.LanMei;
        LoadBullet();
        DoInitEffect(GlobalPosition);
        init = true;
        return true;
    }
    public List<Vector2> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        ds.Add(direction);
        direction = direction.Normalized();
        // 反方向
        float backAngle = direction.Angle() + Mathf.Pi;
        Vector2 backDirection = new Vector2(Mathf.Cos(backAngle), Mathf.Sin(backAngle));
        ds.Add(backDirection);
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

            await ToSignal(GetTree().CreateTimer(0.18f), "timeout");
            __Shoot(bulletScene, startPosition, direction);
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
    }

    public bool Die()
    {
        QueueFree();
        return true;
    }
}
