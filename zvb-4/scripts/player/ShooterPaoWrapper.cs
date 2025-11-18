using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Entity;
using ZVB4.Interface;

public partial class ShooterPaoWrapper : Node2D, IShooterWrapper
{
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
        LoadAttackSpeed(shooterName);
    }
    public IPao pao;
    void _LoadShooterInstance(string shooterName)
    {
        string scenePath = PlansConstants.GetShooterScene(shooterName);
        if (scenePath == string.Empty) return;
        var scene = GD.Load<PackedScene>(scenePath);
        var instance = scene.Instantiate<Node2D>();
        // instance.Name = "Shooter" + shooterName;
        IObj isr = instance as IObj;
        if (isr != null)
        {
            isr.Init(shooterName);
        }
        pao = instance as IPao;
        AddChild(instance);
        //
        if (playerData != null)
        {
            shooterNowName = shooterName;
            SaveDataManager.Instance?.SetPlayerShooter(shooterName);
        }
    }
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
    }
    void Init()
    {
        AttackDirection = Vector2.Up;
        Rotation = 0f;
        Position = InitPosition;
        ResetAttackSpeed();
    }
    public void ChangeShooter(string shooterName)
    {
        try
        {
            shooterNowName = shooterName;
            // 重新加载参数
            _LoadShooterParams(shooterName);
            // 加载射手实力
            _LoadShooterInstance(shooterName);
            // 切换了射手，保存数据
            SaveDataManager.Instance?.SetPlayerShooter(shooterName);
            // 检查是否解锁该射手
            allowAttackNum = PlansConstants.GetShooterAttackLimit(shooterName);
            if (SaveDataManager.Instance == null) return;
            bool hasShooter = (bool)(SaveDataManager.Instance?.IsShooterUnLimit(shooterName));
            if (hasShooter)
            {
                allowAttackNum = -1;
            }
            else
            {
                allowAttackNum = PlayerTool.ComputedShooterShootRatio(allowAttackNum);
            }
        }
        catch
        {
            
        }
    }

    bool CostSunForAttack()
    {
        if (SunCenterSystem.Instance == null) return false;
        return SunCenterSystem.Instance.CostForAttack(shooterCostSun);
    }
    int num = 0;
    public bool AttackAtPosition(Vector2 startPosition, Vector2 direction)
    {
        //
        if (pao == null) return false;
        // 阳光问题
        if (!CostSunForAttack()) return false;
        num += 1;
        // 正式攻击
        pao.ShootBullet(startPosition, direction);
        // 计算次数是否限制
        if (allowAttackNum != -1)
        {
            if (num >= allowAttackNum)
            {
                ShooterWorkTable.Instance?.ChangeToLastBaseShooter();
            }
        }
        //
        SwitchStatus(EnumShooterStatus.Attack);
        return true;
    }

    public void RotateToDirection(Vector2 direction) => throw new NotImplementedException();

    Vector2 workingDirection = Vector2.Zero;
    bool firstAttack = true;
    bool inTouch = false;
    public void Attack(Vector2 attackPos, bool isFirstAttack, Vector2? startPos = null)
    {
        Vector2 pos = startPos == null ? this.GlobalPosition : (Vector2)startPos;
        Vector2 dir = (attackPos - pos).Normalized();
        workingDirection = dir;
        finishedMyRotation = false;
        if (inTouch == false)
        {
            inTouch = true;
            SwitchStatus(EnumShooterStatus.Rotating);
        }
    }
    float __attackZhengLv = 0f;
    float __attackZhengLvLimit = 1f;
    bool __allowAttackOfZhengLv = true;

    float __attackingTime = 0f;
    public override void _Process(double delta)
    {
        try
        {

            RotationEveryFrame(workingDirection, delta);
            // 限制帧率攻击
            if (__attackZhengLv > 0f)
            {
                __attackZhengLv += (float)delta;
                if (__attackZhengLv >= GetAttackZhengLvLimit())
                {
                    __attackZhengLv = 0f;
                    __allowAttackOfZhengLv = true;
                }
            }
            // 切换攻击状态
            if (__attackingTime > 0f)
            {
                __attackingTime += (float)delta;
                if (__attackingTime >= 0.1f)
                {
                    __attackingTime = 0f;
                    if (finishedMyRotation)
                    {
                        SwitchStatus(EnumShooterStatus.Idle);
                    }
                    else
                    {
                        SwitchStatus(EnumShooterStatus.Rotating);
                    }
                }
            }
            // 加载效果
            if (__fireLoadLv > 0f)
            {
                __fireLoadLv += (float)delta;
                if (__fireLoadLv >= __fireLoadLvLimit)
                {
                    __fireLoadLv = 0f;
                    __allowFireLoad = true;
                }
            }
        }
        catch
        {
            
        }
    }
    EnumShooterStatus _shooterStatus = EnumShooterStatus.None;
    void SwitchStatus(EnumShooterStatus status)
    {
        if (_shooterStatus == status) return;
        switch (status)
        {
            case EnumShooterStatus.Idle:
                pao.OnRotingEnd(workingDirection);
                break;
            case EnumShooterStatus.Rotating:
                pao.OnRotingStart(workingDirection);
                break;
            case EnumShooterStatus.Attack:
                __attackingTime = 0.000001f;
                pao.OnRotingEnd(workingDirection);
                pao.OnFireStart(workingDirection);
                TryPlayFireLoadEffect(workingDirection);
                break;
        }
        _shooterStatus = status;
    }
    float __fireLoadLv = 0f;
    float __fireLoadLvLimit = 0.3f;
    bool __allowFireLoad = true;
    void TryPlayFireLoadEffect(Vector2 position)
    {
        if (pao == null) return;
        if (!__allowFireLoad) return;
        __allowFireLoad = false;
        __fireLoadLv = 0.00001f;
        pao.DoFireLoadEffect(position);
    }
    float __speed = 1f;
    float __speedSnap = 0.1f;
    float speedStart = 1f;
    float speedEnd = 0.4f;
    float speedSnapStart = 0.1f;
    float speedSnapEnd = 0.001f;
    float speedSnapSnap = 0.02f;
    void ResetAttackSpeed()
    {
        __speed = speedStart;
        __speedSnap = speedSnapStart;
        _shooterStatus = EnumShooterStatus.Idle;
    }
    void LoadAttackSpeed(string shooterName)
    {
        speedStart = PlansConstants.GetPlansAttackSpeedStart(shooterName);
        speedEnd = PlansConstants.GetPlansAttackSpeedEnd(shooterName);
        speedSnapStart = PlansConstants.GetPlansAttackSpeedSnap(shooterName);
        speedSnapSnap = PlansConstants.GetPlansAttackSpeedSnapSnap(shooterName);
        speedSnapEnd = 0.001f;
        __fireLoadLvLimit = speedEnd * 3;
        RotationSpeed = PlansConstants.GetRationSpeed(shooterName);
        ResetAttackSpeed();
    }
    float GetAttackZhengLvLimit()
    {
        __speed -= __speedSnap;
        if (__speed < speedEnd) __speed = speedEnd;
        __speedSnap -= speedSnapSnap;
        if (__speedSnap < speedSnapEnd) __speedSnap = speedSnapEnd;
        return __speed;
    }

    bool finishedMyRotation = false;
    bool isInRotation = false;
    public void ReleaseAttack(bool mustEmpty = false)
    {
        inTouch = false;
        ResetAttackSpeed();
    }
    float RotationSpeed = 100f; // 每秒旋转速度，度
    void OnRotationArrived(Vector2 dir)
    {
        if (finishedMyRotation) return;
        finishedMyRotation = true;
        isInRotation = false;
        if (__allowAttackOfZhengLv)
        {
            // GD.Print("到达帧率允许时间，发射子弹");
            __allowAttackOfZhengLv = false;
            __attackZhengLv = 0.00001f;
            _attackDirection = dir;
            AttackAtPosition(GlobalPosition, dir);
        }
    }
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
                OnRotationArrived(dir);
            }
            else
            {
                Rotation += Mathf.Sign(angleDiff) * rotateSpeed;
            }
        }
    }

    public void RebuildForBuffs()
    {
        // 攻速调节
        speedEnd = PlansConstants.GetPlansAttackSpeedEnd(shooterNowName);
        speedEnd = PlayerTool.ComputedLowestAttackSpeedRatio(speedEnd);
    }


    private Vector2 _attackDirection = Vector2.Zero;
    public Vector2 AttackDirection
    {
        get => _attackDirection;
        set => _attackDirection = value;
    }
}
