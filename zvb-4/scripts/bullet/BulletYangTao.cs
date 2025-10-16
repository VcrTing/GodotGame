using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletYangTao : Node2D, IBulletBase, IObj, IAttack
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
        Damage = BulletConstants.GetDamage(objName);

        //
        AnimatedSprite2D view = GodotTool.GetViewAndAutoPlay(this);
        _area2D = GetNode<Area2D>(NameConstants.AttackArea);
        if (_area2D != null)
        {
            _area2D.AreaEntered += OnArea2DAreaEntered;
        }
        // 启动计时器变量，Process里计时
        _autoDieElapsed = 0f;
        _autoDieActive = true;
    }

    // 用于Process计时的自动销毁
    private float _autoDieElapsed = 0f;
    private bool _autoDieActive = false;

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

    [Export]
    public bool isRotate = true;
    // 渐隐相关变量
    float fadeElapsed = 0f;
    float fadeDuration = AnimationConstants.BulletFadeDieDuration;
    float fadeLowest = 0f;
    public override void _Process(double delta)
    {
        // 自动死亡计时
        if (_autoDieActive && !isDoDie)
        {
            _autoDieElapsed += (float)delta;
            if (_autoDieElapsed >= BulletConstants.LiveTimeTotal)
            {
                _autoDieActive = false;
                DoDie();
            }
        }

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
        
        if (isRotate)
        {
            // GGG: 自身慢速旋转
            Rotation += Mathf.DegToRad(60f) * (float)delta; // 每秒60度
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

    public bool DoTakeDamage(Area2D area) => ObjTool.TakeDamage(area, objType, Damage, hurtType);

    public float Speed { get; set; } = BulletConstants.SpeedBasic; // 默认速度
    float SpeedInit = BulletConstants.SpeedBasic; // 初始速度
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

    string objName = BulletConstants.BulletYangTaoName;
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

    public int GetDamage() => BulletConstants.GetDamage(objName);
    int Damage;
    public int GetDamageExtra() => BulletConstants.GetDamageExtra(objName);

    public Vector2 GetDirection() => Direction;

    public bool CanAttack()
    {
        throw new NotImplementedException();
    }

}
