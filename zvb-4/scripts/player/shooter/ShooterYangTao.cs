using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterYangTao : Node2D, IObj, IShooter
{
    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;

    [Export]
    public string ObjName { get; set; } = PlansConstants.YangTao;
    public string GetObjName() => ObjName;

    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.YangTao;
        LoadBullet();
        DoInitEffect(GlobalPosition);
        init = true;
        return true;
    }

    public List<Vector2> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        direction = direction.Normalized();
        ds.Add(direction);
        // MMM: 计算五角星的其他4个方向
        float angle = direction.Angle();
        float starStep = Mathf.Pi * 2f / 5f; // 72度
        for (int i = 1; i < 5; i++)
        {
            float newAngle = angle + starStep * i;
            Vector2 dir = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
            ds.Add(dir);
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
        BulletScenePath = PlansConstants.GetBullet(ObjName);
    }

    public void DoFireEffect(Vector2 position)
    {
        // 播放音效
        SoundPlayerController.Instance?.EnqueueSound("Bullets/" + ObjName, ObjName, 4);
    }

    public void DoInitEffect(Vector2 position)
    {
        // 播放音效
        SoundFxController.Instance.PlayFx("Fx/upshooter", "up", 4);
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
