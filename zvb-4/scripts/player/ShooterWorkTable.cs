using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterWorkTable : Node2D
{
    public static ShooterWorkTable Instance { get; private set; }
    StaticBody2D workTableBody;
    CapsuleShape2D rect;
    public override void _Ready()
    {
        Instance = this;
        GetBodtRect();
        GodotTool.GetViewAndAutoPlay(this);
    }

    void GetBodtRect()
    {
        workTableBody = GetNodeOrNull<StaticBody2D>(NameConstants.ShooterWorkTableBody);
        if (workTableBody != null)
        {
            var shapeNode = workTableBody.GetNodeOrNull<CollisionShape2D>(NameConstants.CollisionShape2D);
            if (shapeNode != null && shapeNode.Shape is CapsuleShape2D r)
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
        /*
        if (plansNameNow == plansName)
        {
            Node2D node2D = GodotTool.FindNode2DByName(this.GetParent(), NameConstants.ShooterWrapper);
            DieToReword(node2D);
            return true;
        }
        */
        return ChangeShooter(plansName);
    }

    public bool ChangeToLastBaseShooter()
    {
        if (SaveDataManager.Instance == null) return false;
        string lastBaseShooter = SaveDataManager.Instance?.GetLastBaseShooter();
        return ChangeShooter(lastBaseShooter);
    }

    public bool ChangeShooter(string plansName)
    {
        var pc = PlayerController.Instance;
        if (pc == null) return false;
        pc.UnLockAttack();
        //
        string shooterScene = PlansConstants.GetShooterWrapperScenePath(plansName);
        if (!string.IsNullOrEmpty(shooterScene))
        {
            var scene = GD.Load<PackedScene>(shooterScene);
            if (scene == null) return false;
            plansNameNow = plansName;
            SaveDataManager.Instance?.TrySavePlayerShooterBaseLast(plansNameNow);
            // 删掉老射手。
            PlayerController.Instance?.TrashOldShooter();
            GenerateShooter(scene);
            return true;
        }
        else
        {
            SoundUiController.Instance?.Error();
        }
        return false;
    }

    void GenerateShooter(PackedScene scene)
    {
        //
        var shooterInstance = scene.Instantiate();
        shooterInstance.Name = NameConstants.ShooterWrapper;
        AddChild(shooterInstance);
        if (shooterInstance is IShooterWrapper s)
        {
            s.ChangeShooter(plansNameNow);
        }
        // 删掉花盆
    }  
}
