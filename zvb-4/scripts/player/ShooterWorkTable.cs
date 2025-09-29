using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterWorkTable : Node2D
{
    public static ShooterWorkTable Instance { get; private set; }
    StaticBody2D workTableBody;
    CircleShape2D rect;
    public override void _Ready()
    {
        Instance = this;
        //
        GetBodtRect();
    }

    void GetBodtRect()
    {
        workTableBody = GetNodeOrNull<StaticBody2D>(NameConstants.ShooterWorkTableBody);
        if (workTableBody != null)
        {
            var shapeNode = workTableBody.GetNodeOrNull<CollisionShape2D>(NameConstants.CollisionShape2D);
            if (shapeNode != null && shapeNode.Shape is CircleShape2D r)
            {
                rect = r;
            }
        }
    }

    public bool IsInMe(Vector2 pos)
    {
        GetBodtRect();
        if (workTableBody != null && rect != null)
        {
            // 转换pos到body的本地坐标
            var localPos = workTableBody.ToLocal(pos);
            // 判断是否在圆形内
            bool inside = localPos.Length() <= rect.Radius;
            return inside;
        }
        return false;
    }

    string plansNameNow = string.Empty;

    public bool HandleCollision(string plansName)
    {
        if (plansNameNow == plansName)
        {
            // 开启可攻击
            PlayerController.Instance?.SetCanAttack(true);
            return true;
        }
        else
        {
            string shooterScene = PlansConstants.GetShooterScene(plansName);
            if (!string.IsNullOrEmpty(shooterScene))
            {
                plansNameNow = plansName;
                ZhongXiaShooter(plansName, shooterScene);
                return true;
            }
            else
            {
                SoundUiController.Instance?.Error();
            }
        }
        return false;
    }

    void ZhongXiaShooter(string shooterName, string shooterScene)
    {
        // 生成shooter实例
        var scene = GD.Load<PackedScene>(shooterScene);
        if (scene == null)
        {
            return;
        }

        // 删掉老射手。
        PlayerController.Instance?.TrashOldShooter();
        //
        var shooterInstance = scene.Instantiate();
        shooterInstance.Name = NameConstants.Shooter;
        AddChild(shooterInstance);
        if (shooterInstance is IShooter shooterInterface)
        {
            shooterInterface.ChangeShooter(shooterName);
            // 开启可攻击
            PlayerController.Instance?.SetCanAttack(true);
        }
            
    }
}
