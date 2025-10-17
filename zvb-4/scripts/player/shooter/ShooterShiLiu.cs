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
    int deg = 10; // 7 + 3
    // 散射
    public List<ShootBulletRequest> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<ShootBulletRequest> ds = new List<ShootBulletRequest>();
        ds.Add(new ShootBulletRequest { startPosition = startPosition, direction = direction });
        ds.Add(new ShootBulletRequest { startPosition = GameTool.OffsetPosition(startPosition, 4), direction = GameTool.RotateVector2(direction, 4) });
        ds.Add(new ShootBulletRequest { startPosition = GameTool.OffsetPosition(startPosition, -4), direction = GameTool.RotateVector2(direction, -4) });
        return ds;
    }
    // 自射
    public List<ShootBulletRequest> ComputedAttackDirectionsExtra(Vector2 startPosition, Vector2 direction)
    {
        List<ShootBulletRequest> ds = new List<ShootBulletRequest>();
        // ds.Add(new ShootBulletRequest { startPosition = startPosition, direction = direction });
        for (float d = -9; d <= 9; d += 3f)
        {
            Vector2 pos = GameTool.OffsetPosition(startPosition, d * 3);
            ds.Add(new ShootBulletRequest { startPosition = pos, direction = GameTool.RotateVector2(direction, d) });
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
            List<ShootBulletRequest> directions = ComputedAttackDirections(startPosition, direction);
            foreach (var dir in directions)
            {
                __Shoot(bulletScene, dir.startPosition, dir.direction);
            }
            // 播放音效
            DoFireEffect(startPosition);
            //
            await ToSignal(GetTree().CreateTimer(0.04f), "timeout");
            directions = ComputedAttackDirectionsExtra(startPosition, direction);
            foreach (var dir in directions)
            {
                __Shoot(bulletScene, dir.startPosition, dir.direction);
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
