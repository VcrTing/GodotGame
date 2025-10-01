using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletZero : Node2D, IBulletBase, IObj, IAttack
{
    private Area2D _area2D;

    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        //
        AdjustView();
        //
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;

        //
        AnimatedSprite2D view = GodotTool.GetViewAndAutoPlay(this);
        _area2D = GetNode<Area2D>(NameConstants.AttackArea);
        if (_area2D != null)
        {
            _area2D.AreaEntered += OnArea2DAreaEntered;
        }
        // 
        _ = AutoDie();
    }

    private async Task AutoDie()
    {
        await ToSignal(GetTree().CreateTimer(BulletConstants.LiveTimeTotal), "timeout");
        DoDie();
    }

    private bool _hasHit = false;
    private void OnArea2DAreaEntered(Area2D area)
    {
        if (_hasHit) return; // 只处理第一个碰撞体
        bool isOk = DoTakeDamage(area);
        if (isOk)
        {
            _hasHit = true; DieWhenHit();
        }
    }

    public void MoveBullet(Vector2 direction, float speed, double delta)
    {
        Position += direction * speed * (float)delta;
    }

    // 渐隐相关变量
    float fadeElapsed = 0f;
    float fadeDuration = AnimationConstants.BulletFadeDieDuration;
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
        if (cs <= (SpeedInit * 0.1f))
        {
            DoDie();
        }
        return cs;
    }

    public bool DoTakeDamage(Area2D area)
    {
        bool isWorking = false;
        if (area is IHurtBase hurtArea)
        {
            isWorking = hurtArea.TakeDamage(objType, Damage, hurtType);
        }
        else
        {
            var parent = area.GetParent();
            if (parent is IHurtBase hurt)
            {
                isWorking = hurt.TakeDamage(objType, Damage, hurtType);
            }
        }
        return isWorking;
    }
    public float Speed { get; set; } = BulletConstants.SpeedPea; // 默认速度
    float SpeedInit = BulletConstants.SpeedPea; // 初始速度
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.Pea;
    public EnumHurts GetHurtType() => hurtType;
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上

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
        if (_area2D != null) _area2D.QueueFree();
        var view = GetNodeOrNull<CanvasItem>(NameConstants.View);
        view.Visible = false;
        __Die();
    }
    private void DieOfFade()
    {
        if (_area2D != null) _area2D.QueueFree();
        __Die();
    }

    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;

    string objName = BulletConstants.BulletPeaName;
    public string GetObjName() => objName;

    public bool Init(string name = null)
    {
        return true;
    }

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

    private int Damage = BulletConstants.DamagePea;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => 0;

    public Vector2 GetDirection() => Direction;
}
