using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterLanMei : Node2D, IObj, IShooter
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
    float deg = 7f;
    public List<Vector2> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        direction = direction.Normalized();
        // NNN: 计算direction左边偏8度和右边偏8度的方向
        float angleRad = Mathf.DegToRad(deg);
        // 左偏8度
        float leftAngle = direction.Angle() - angleRad;
        Vector2 leftDirection = new Vector2(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle));
        ds.Add(leftDirection);
        // 右偏8度
        float rightAngle = direction.Angle() + angleRad;
        Vector2 rightDirection = new Vector2(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle));
        ds.Add(rightDirection);
        //
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

    public void ShootBullet(Vector2 startPosition, Vector2 direction)
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
