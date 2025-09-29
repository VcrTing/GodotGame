using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Entity;
using ZVB4.Interface;

public partial class Shooter : Node2D, IShooter
{

    EntityPlayerData playerData = null;
    string shooterNowName;
    int shooterCostSun;
    Vector2 InitPosition;

    public override void _Ready()
    {
        // GD.Print($"Shooter position: {Position}");
        InitPosition = Position;
        // 
        LoadShooter();
    }

    void LoadShooter()
    {
        // 获取玩家数据
        try
        {
            playerData = SaveDataManager.Instance?.GetPlayerData();
        }
        catch
        {

        }
        if (playerData == null)
        {
            // 延迟0.2f后重试
            var _ = DelayAndReload();
            return;
        }
        // 获取当前射手
        shooterNowName = playerData.ShooterNow;
        // 
        _LoadNow(shooterNowName);
    }
    async System.Threading.Tasks.Task DelayAndReload()
    {
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        LoadShooter();
    }
    void _LoadNow(string shooterName)
    {
        shooterNowName = shooterName;
        shooterCostSun = SunMoneyConstants.GetPlansSunCost(shooterName);
        // 获取射手对应的子弹实例
        bulletScenePath = PlansConstants.GetBullet(shooterName);
    }

    string bulletScenePath = string.Empty;

    public void ChangeShooter(string shooterName)
    {
        // 先隐藏所有子节点
        foreach (Node child in GetChildren())
        {
            if (child is CanvasItem canvasItem)
                canvasItem.Visible = false;
        }
        // 再显示指定射手子节点
        var shooterNode = GetNodeOrNull<Node2D>(shooterName);
        if (shooterNode is CanvasItem shooterCanvas)
        {
            shooterCanvas.Visible = true;
        }
        _LoadNow(shooterName);
        // 切换了射手，保存数据
        SaveDataManager.Instance?.SetPlayerShooter(shooterName);
    }

    bool CheckAttackSun()
    {
        if (bulletScenePath == string.Empty) return false;

        if (playerData == null) return false;
        try
        {
            bool isZero = SunCenterSystem.Instance.NextSunIsZero(shooterCostSun);
            if (isZero)
            {
                // 错误音效
                SoundUiController.Instance?.Error();
                return false;
            }
            else
            {
                // 扣除阳光
                SunCenterSystem.Instance?.ForAttack(shooterCostSun);
            }
            return true;
        }
        catch
        {

        }
        return false;
    }

    public void AttackAtPosition(Vector2 startPosition, Vector2 direction)
    {
        // 阳光问题
        if (!CheckAttackSun()) return;
        // 正式攻击
        var bulletScene = GD.Load<PackedScene>(bulletScenePath);
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
            SoundPlayerController.Instance?.EnqueueSound("Bullets/" + shooterNowName, shooterNowName, 4);
        }
    }

    public void RotateToDirection(Vector2 direction)
    {
        // 默认朝向为上，旋转到目标方向，加90度
        Rotation = direction.Angle() + Mathf.Pi / 2;
    }
}
