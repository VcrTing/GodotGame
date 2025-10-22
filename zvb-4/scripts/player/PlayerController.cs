using System.Collections.Generic;
using Godot;
using ZVB4.Conf;
using ZVB4.Entity;
using ZVB4.Interface;


public partial class PlayerController : Node2D
{
    [Export]
    public bool IsInitShooter = false;
    [Export]
    public EnumPlayerShooterMode ShooterMode = EnumPlayerShooterMode.TouchShooter;

    // 射击限制
    float ShooterShootRatio = 1;
    public void SetShootRatio(float ratio) => ShooterShootRatio = ratio;
    public float GetShootRatio() => ShooterShootRatio;
    //
    public static PlayerController Instance { get; private set; }
    public Vector2 _lastClickPosition;
    private IShooterWrapper _shooter;
    Node2D workTable;
    public Vector2 _shooterInitPosition;
    public override void _Ready()
    {
        Instance = this;
        InitShooterWorkTable();
    }
    void InitShooterWorkTable() {
        workTable = PlayerTool.GenerateWorkTable(ShooterMode, this);
    }
    public async void LoadInitShooter(string shooterName)
    {
        if (IsInitShooter == false) return;
        TrashOldShooter();
        if (workTable == null)
        {
            InitShooterWorkTable();
            GetTree().CreateTimer(0.1f).Timeout += () => LoadInitShooter(shooterName);
            return;
        }
        ShooterWorkTable wt = workTable as ShooterWorkTable;
        wt.HandleCollision(shooterName);
        AfterGetShooter();
        RebuildBuffs();
    }
    
    public void TrashOldShooter()
    {
        try
        {
            Node2D nd = GodotTool.FindNode2DByName(this, NameConstants.ShooterWrapper);
            if (nd == null) return;
            nd.Name = NameConstants.ShooterWrapper + "_to_be_deleted";
            nd.QueueFree();
            _shooter = null;
        }
        catch { }
    }
    void AfterGetShooter()
    {
        try
        {
            Node2D nd = GodotTool.FindNode2DByName(this, NameConstants.ShooterWrapper);
            if (nd == null) return;
            if (nd is IShooterWrapper) { _shooter = nd as IShooterWrapper; }
            if (_shooter != null) { _shooterInitPosition = nd.GlobalPosition; }
        }
        catch { }
    }
    Vector2 lastPos = Vector2.Zero;
    public void SlideLocation(Vector2 pos)
    {
        if (pos.X == 0) {
            // GD.Print("滑动到中心位置");
        }
        Vector2 ps = new Vector2(pos.X, Position.Y);
        lastPos = ps;
    }

    //
    bool canAttack = true;
    public void ForbiddenAttack()
    {
        canAttack = false;
        ReleaseAttackClear(__lastClickPos, true);
    }
    public void UnLockAttack()
    {
        canAttack = true;
        ReleaseAttackClear(__lastClickPos, true);
    }
    //
    void Attack(Vector2 clickPosition, bool isFirstAttack)
    {
        if (canAttack == false) return;
        if (_shooter == null) AfterGetShooter();
        if (_shooter == null) return;
        _shooter.Attack(clickPosition, isFirstAttack);
    }
    void Attack(Vector2 clickPosition, bool isFirstAttack, Vector2 startPosition)
    {
        if (canAttack == false) return;
        if (_shooter == null) AfterGetShooter();
        if (_shooter == null) return;
        Vector2 s = new Vector2(clickPosition.X, clickPosition.Y + 50);
        _shooter.Attack(clickPosition, isFirstAttack, s);
    }
    //
    bool __startAttack = false;
    Vector2 __lastClickPos = Vector2.Zero;
    public void TryAttack(Vector2 clickPosition)
    {
        __startAttack = true;
        __lastClickPos = clickPosition;
    }
    public void ReleaseAttack(Vector2 clickPosition)
    {
        ReleaseAttackClear(clickPosition, false);
    }
    public void ReleaseAttackClear(Vector2 clickPosition, bool isClear = true)
    {
        __lastClickPos = clickPosition;
        __startAttack = false;
        firstAttack = false;
        __attackTime = 0;
        if (_shooter == null) return;
        _shooter?.ReleaseAttack(isClear);
    }
    public float touchShortPressTime = 0.02f; // 长按时间，秒
    float __attackTime = 0;
    bool firstAttack = false;
    // 
    public override void _Process(double delta)
    {

        Position = new Vector2(lastPos.X, Position.Y);
        if (ShooterMode == EnumPlayerShooterMode.TouchShooter)
        {
            RunningTouchShooter(delta);
        }
        else
        {
            RunningLineShooter(delta);
        }

    }

    // 玩家攻速调节
    float __lowestAttackSpeedRatio = 1f;
    float InitialLowestAttackSpeedRatio = 1f;
    public float GetLowestAttackSpeedRatio() => __lowestAttackSpeedRatio;
    public void SetLowestAttackSpeedRatio(float ratio) {
        InitialLowestAttackSpeedRatio = ratio;
        __lowestAttackSpeedRatio = ratio;
    }
    // 玩家伤害调节
    float __attackDamageRatio = 1f;
    float InitialAttackDamageRatio = 1f;
    public float GetAttackDamageRatio() => __attackDamageRatio;
    public void SetAttackDamageRatio(float ratio)
    {
        InitialAttackDamageRatio = ratio;
        __attackDamageRatio = ratio;
    }
    // 重置调节
    public void ResetAttackAdjust()
    {
        __lowestAttackSpeedRatio = InitialLowestAttackSpeedRatio;
        __attackDamageRatio = InitialAttackDamageRatio;
        _shooter?.RebuildForBuffs();
    }
    List<ShootBuff> buffList = new List<ShootBuff>();
    // Buff 重建
    void RebuildBuffs()
    {
        // 攻速 Buff
        __lowestAttackSpeedRatio = InitialLowestAttackSpeedRatio;
        // 伤害 Buff
        __attackDamageRatio = InitialAttackDamageRatio;
        foreach (var buff in buffList)
        {
            __lowestAttackSpeedRatio = PlayerTool.ComputedSpeedBuffV(__lowestAttackSpeedRatio, buff.attackSpeedRatio);
            
            //
            __attackDamageRatio *= buff.attackDamageRatio;
        }
        // 调整射手属性
        _shooter?.RebuildForBuffs();
    }
    public void AddShootBuff(ShootBuff buff)
    {
        buffList.Add(buff);
        RebuildBuffs();
    }
    public void RemoveShootBuff(ShootBuff buff)
    {
        buffList.Remove(buff);
        RebuildBuffs();
    }

    void RunningLineShooter(double delta)
    {
        Attack(Vector2.Up, false, Position);
    }
    
    void RunningTouchShooter(double delta)
    {
        if (__startAttack)
        {
            __attackTime += (float)delta;

            if (firstAttack)
            {
                __attackTime = 0;
                Attack(__lastClickPos, false);
            }
            else
            {
                if (__attackTime >= touchShortPressTime)
                {
                    firstAttack = true;
                    __attackTime = 0;
                    Attack(__lastClickPos, true);
                }
            }
        }
    }
}
