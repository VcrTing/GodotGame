using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Entity;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ShooterGunWrapper : Node2D, IShooterWrapper
{

    EntityPlayerData playerData = null;
    string shooterNowName;
    int shooterCostSun;
    Vector2 InitPosition;

    int allowAttackNum = 0;

    public override void _Ready()
    {
        InitPosition = Position;
        Init();
        LoadData();
        // LoadShooter();
    }

    public void ChangeShooter(string shooterName)
    {
        // 加载射手实力
        _LoadShooterInstance(shooterName);
        // 重新加载参数
        _LoadShooterParams(shooterName);
        // 切换了射手，保存数据
        SaveDataManager.Instance?.SetPlayerShooter(shooterName);
        // 检查是否解锁该射手
        allowAttackNum = PlansConstants.GetShooterAttackLimit(shooterNowName);
        if (SaveDataManager.Instance == null) return;
        bool hasShooter = (bool)(SaveDataManager.Instance?.HasThisShooter(shooterNowName));
        if (hasShooter)
        {
            allowAttackNum = -1;
        }
        else
        {
            allowAttackNum = PlayerTool.ComputedShooterShootRatio(allowAttackNum);
        }
    }

    bool CostSunForAttack()
    {
        if (SunCenterSystem.Instance == null) return false;
        return SunCenterSystem.Instance.CostForAttack(shooterCostSun);
    }

    public bool AttackAtPosition(Vector2 startPosition, Vector2 direction)
    {
        //
        if (shooter == null) return false;
        // 阳光问题
        if (!CostSunForAttack()) return false;
        num += 1;
        // 正式攻击
        shooter.ShootBullet(startPosition, direction);
        // 计算次数是否限制
        if (allowAttackNum != -1)
        {
            if (num >= allowAttackNum)
            {
                ShooterWorkTable.Instance?.ChangeToLastBaseShooter();
            }
        }
        return true;
    }

    public void RotateToDirection(Vector2 direction)
    {
        // 默认朝向为上，旋转到目标方向，加90度
        Rotation = direction.Angle() + Mathf.Pi / 2;
    }
    private Vector2 _attackDirection = Vector2.Zero;
    public Vector2 AttackDirection
    {
        get => _attackDirection;
        set => _attackDirection = value;
    }
    // 
    public void Init()
    {
        attackNum = 0;
        __attackFlagTime = 0f;
        AttackDirection = Vector2.Up;
        Rotation = 0f;
        Position = InitPosition;
        NowRotationDir = Vector2.Up;
        ResetAttackModel();
    }
    public void ReleaseAttack(bool mustEmpty = false)
    {
        if (attackDirectionList.Count > 0)
        {
            if (mustEmpty)
            {
                attackDirectionList.Clear();
            }
            // 只保留最后一个方向
            else
            {
                AttackDirection = attackDirectionList[attackDirectionList.Count - 1];
                attackDirectionList.Clear();
                RotateToDirection(AttackDirection);
                AttackAtPosition(GlobalPosition, AttackDirection);
            }
            attackNum = 0;
            __attackFlagTime = 0f;
            ResetAttackModel();
        }
        else
        {
            attackNum = 0;
            __attackFlagTime = 0f;
            ResetAttackModel();
        }
    }

    List<Vector2> attackDirectionList = new List<Vector2>();
    bool addAttackDirectionFlag = false;
    int attackNum = 0;
    public void Attack(Vector2 attackPos, bool isFirstAttack, Vector2? startPos = null)
    {
        // 计算方向
        Vector2 pos = startPos == null ? this.GlobalPosition : (Vector2)startPos;
        Vector2 dir = (attackPos - pos).Normalized();
        // 首次攻击放行
        if (isFirstAttack)
        {
            attackNum = 0;
            __attackFlagTime = 0f;
            RotateToDirection(dir);
            AttackAtPosition(pos, dir);
            ResetAttackModel();
        }
        else
        {
            NowRotationDir = dir;
            attackNum += 1;
        }
        if (addAttackDirectionFlag)
        {
            addAttackDirectionFlag = false;
            attackDirectionList.Add(dir);
        }
    }

    float __attackFlagTime = 0f;
    float __attackTime = 0f;

    int num = 0;
    bool succAttack = true;
    public override void _Process(double delta)
    {
        if (attackNum > 0)
        {
            __attackFlagTime += (float)delta;
            if (__attackFlagTime >= __attackSpeed)
            {
                addAttackDirectionFlag = true;
                __attackFlagTime = 0f;
            }

            //
            __attackTime += (float)delta;
            if (__attackTime >= __attackSpeed)
            {
                // 
                if (attackDirectionList.Count > 0)
                {
                    Vector2 one = attackDirectionList[0];
                    AttackDirection = one;
                    attackDirectionList.RemoveAt(0);
                    RotateToDirection(AttackDirection);
                    succAttack = AttackAtPosition(this.GlobalPosition, AttackDirection);
                }
            }
            if (succAttack) { RunnerAttackInterval(delta); }
        }
        RotationEveryFrame(NowRotationDir, delta);
    }
    float attackSpeedStart = 0.3f;
    float speedEnd = 0.3f;
    float attackSpeedSnap = 0.05f;
    float attackSpeedSnapSnap = 0.005f;

    float __attackSpeedSnapLowest = 0.005f;
    float __attackSpeed = 0.3f;
    float __attackSpeedSnap = 0.05f;
    float __attackSnapTime = 0f;
    // 每次攻击间隔
    void ResetAttackModel()
    {
        __attackSpeed = attackSpeedStart;
        __attackSnapTime = 0f;
        __attackSpeedSnap = attackSpeedSnap;
        succAttack = true;
    }
    float RunnerAttackInterval(double delta)
    {
        __attackSnapTime += (float)delta;
        if (__attackSnapTime >= __attackSpeed)
        {
            __attackSnapTime = 0f;
            __attackSpeed -= __attackSpeedSnap;
            if (__attackSpeed <= speedEnd)
            {
                __attackSpeed = speedEnd;
            }
            // 曲线降速
            __attackSpeedSnap -= attackSpeedSnapSnap;
            if (__attackSpeedSnap <= __attackSpeedSnapLowest)
            {
                __attackSpeedSnap = __attackSpeedSnapLowest;
            }
        }
        return __attackSpeed;
    }
    float RotationSpeed = 100f; // 每秒旋转100度
    Vector2 NowRotationDir = Vector2.Up;
    void RotationEveryFrame(Vector2 dir, double delta)
    {
        if (dir != Vector2.Zero)
        {
            float targetAngle = dir.Angle() + Mathf.Pi / 2;
            float currentAngle = Rotation;
            // 计算每帧旋转速度（10度/0.1s）
            float rotateSpeed = Mathf.DegToRad(RotationSpeed / 10) / 0.1f * (float)delta; // 每秒100度
            float angleDiff = Mathf.Wrap(targetAngle - currentAngle, -Mathf.Pi, Mathf.Pi);
            if (Mathf.Abs(angleDiff) < rotateSpeed)
            {
                Rotation = targetAngle;
                OnRotationArrived(targetAngle);
            }
            else
            {
                Rotation += Mathf.Sign(angleDiff) * rotateSpeed;
            }
        }
    }
    // 旋转到达目标方向时调用
    void OnRotationArrived(float targetAngle)
    {
        // TODO: 实现到达目标方向后的逻辑
    }

    // 加载当前射手
    void LoadData()
    {
        if (SaveDataManager.Instance == null)
        {
            var _ = DelayAndReload();
            return;
        }
        playerData = SaveDataManager.Instance.GetPlayerData();
        if (playerData == null)
        {
            var _ = DelayAndReload();
            return;
        }
    }
    async System.Threading.Tasks.Task DelayAndReload()
    {
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        LoadData();
    }
    void _LoadShooterParams(string shooterName)
    {
        if (shooterName == string.Empty) return;
        shooterCostSun = SunMoneyConstants.GetPlansSunCost(shooterName);
        // 获取攻击速度
        attackSpeedStart = PlansConstants.GetPlansAttackSpeedStart(shooterName);
        speedEnd = PlansConstants.GetPlansAttackSpeedEnd(shooterName);
        attackSpeedSnap = PlansConstants.GetPlansAttackSpeedSnap(shooterName);
        __attackSpeed = attackSpeedStart;
        attackSpeedSnapSnap = PlansConstants.GetPlansAttackSpeedSnapSnap(shooterName);
        __attackSpeedSnap = attackSpeedSnap;
        RebuildForBuffs();
    }
    public IShooter shooter;
    void _LoadShooterInstance(string shooterName)
    {
        string scenePath = PlansConstants.GetShooterScene(shooterName);
        if (scenePath == string.Empty) return;
        var scene = GD.Load<PackedScene>(scenePath);
        var instance = scene.Instantiate<Node2D>();
        // instance.Name = "Shooter" + shooterName;
        IObj isr = instance as IObj;
        if (isr != null) { isr.Init(shooterName); }
        shooter = instance as IShooter;
        AddChild(instance);
        if (playerData != null)
        {
            shooterNowName = shooterName;
            SaveDataManager.Instance?.SetPlayerShooter(shooterName);
        }
    }

    public void RebuildForBuffs()
    {
        // 攻速调节
        speedEnd = PlansConstants.GetPlansAttackSpeedEnd(shooterNowName);
        speedEnd = PlayerTool.ComputedLowestAttackSpeedRatio(speedEnd);
    }
}
