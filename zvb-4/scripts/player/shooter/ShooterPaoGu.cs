using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterPaoGu : Node2D, IShooter, IObj
{
    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;

    [Export]
    public string ObjName { get; set; } = PlansConstants.PaoGu;
    public string GetObjName() => ObjName;

    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.PaoGu;
        LoadBullet();
        init = true;
        DoInitEffect(GlobalPosition);
        return true;
    }

    public void ShootBullet(Vector2 startPosition, Vector2 direction)
    {
        // 正式攻击
        var bulletScene = GD.Load<PackedScene>(BulletScenePath);
        // GD.PrintErr("ShooterPea ShootBullet: " + BulletScenePath);
        if (bulletScene != null)
        {
            var bullet = bulletScene.Instantiate<Node2D>();
            bullet.Position = startPosition;
            // 调整运动方向
            if (bullet is IBulletBase bulletBase)
            {
                bulletBase.SetDirection(direction.Normalized());
            }
            GetTree().CurrentScene.AddChild(bullet);
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

    public void DoRotingEffect(Vector2 direction)
    {
        throw new NotImplementedException();
    }

}
