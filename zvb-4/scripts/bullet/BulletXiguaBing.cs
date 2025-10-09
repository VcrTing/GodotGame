using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletXiguaBing : Node2D, IObj, IBulletBase, IAttack, IWorking
{
    CanvasItem view = null;
    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        Init();
        view = GodotTool.FindNode2DByName(this, NameConstants.View);        
        // MMM
        //
        AdjustView();
        // 
        _ = AutoDie();
    }

    private async Task AutoDie()
    {
        await ToSignal(GetTree().CreateTimer(BulletConstants.LiveTimeTotal), "timeout");
        DoDie();
    }

    bool canMove = true;
    public void MoveBullet(Vector2 direction, float speed, double delta)
    {
        if (!canMove) return;
        Position += direction * speed * (float)delta;
    }

    // 渐隐相关变量
    float fadeElapsed = 0f;
    float fadeDuration = AnimationConstants.BulletXiGuaFadeDieDuration;
    float fadeLowest = 0f;
    public override void _Process(double delta)
    {
        Speed = FixSpeedByY();
        MoveBullet(Direction, Speed, delta);
        if (isDoDie)
        {
            AdjustHorizion();
            // 死亡效果
            RunningDieFx(delta);
        }
        else
        {
            AdjustView();
        }
    }
    void RunningDieFx(double delta)
    {
        GameTool.RunnerBulletZeroWhenDieFx(this, delta, ref fadeElapsed, fadeDuration, fadeLowest);
    }
    bool isDoDie = false;
    void DoDie()
    {
        if (!isDoDie)
        {
            isDoDie = true;
            DieOfFade();
        }
    }
    float maxY = GameContants.HorizonBulletY; // 子弹出界的y值
    float FixSpeedByY()
    {
        float cs = GameTool.FixBulletSpeedByY(SpeedInit, Position, maxY);
        if (cs <= 10f)
        {
            DoDie();
        }
        return cs;
    }

    public float Speed { get; set; } = 0; // 默认速度
    float SpeedInit = 0; // 初始速度
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上
    public Vector2 GetDirection() => Direction;
    public void SetDirection(Vector2 direction) => Direction = direction;
    public void FlipXDirection() => Direction = new Vector2(-Direction.X, Direction.Y);
    public void FlipYDirection() => Direction = new Vector2(Direction.X, -Direction.Y);

    async void __Die()
    {
        await ToSignal(GetTree().CreateTimer(fadeDuration), "timeout");
        QueueFree();
    }
    void DieWhenHit()
    {
        __Die();
    }
    private void DieOfFade()
    {
        __Die();
    }

    [Export]
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    string objName = BulletConstants.BulletXiguaBingName;
    public string GetObjName() => objName;
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.XiguaBing;
    public EnumHurts GetHurtType() => hurtType;


    public bool Die()
    {
        throw new NotImplementedException();
    }

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustHorizion()
    {
        ViewTool.ViewZiFace(this);
        return true;
    }
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        return true;
    }

    private int Damage = 0;
    private int DamageExtra = 0;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => DamageExtra;
    //
    public bool Init(string name = null)
    {
        Damage = BulletConstants.GetDamage(objName);
        DamageExtra = BulletConstants.GetDamageExtra(objName);
        // GD.Print($"BulletXiguaBing Init: {objName}, Damage: {Damage}, DamageExtra: {DamageExtra}");
        //
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;
        return true;
    }

    //
    public void StartAttack()
    {
        // 开启攻击范围
        BulletXiguaBingAttackArea area = GetNodeOrNull<BulletXiguaBingAttackArea>(NameConstants.AttackArea);
        if (area != null)
        {
            area.Init();
        }
        // 暂停父类移动
        Speed = 0;
        // 开启 溢出攻击范围
        BulletXiguaBingAttackExtraArea extraArea = GetNodeOrNull<BulletXiguaBingAttackExtraArea>(NameConstants.AttackExtraArea);
        if (extraArea != null)
        {
            extraArea.Init();
        }
    }

    bool isWorkingMode = false;
    public void SetWorkingMode(bool working)
    {
        isWorkingMode = working;
        if (working)
        {
            StartAttack();
            DieOfFade();
            canMove = false;
        }
    }

    public bool IsWorking() => isWorkingMode;
}
