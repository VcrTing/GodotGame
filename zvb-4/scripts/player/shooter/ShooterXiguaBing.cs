using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterXiguaBing : Node2D, IObj, IShooter
{
    
    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;

    [Export]
    public string ObjName { get; set; } = PlansConstants.XiguaBing;
    public string GetObjName() => ObjName;

    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.XiguaBing;
        LoadBullet();
        DoInitEffect(GlobalPosition);
        init = true;
        return true;
    }

    public void ShootBullet(Vector2 startPosition, Vector2 direction)
    {
        // 正式攻击
        var bulletScene = GD.Load<PackedScene>(BulletScenePath);
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
