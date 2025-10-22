using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletZeroCold : Node2D, IBulletBase, IObj, IAttack, IWorking
{
    [Export]
    public bool IsManyDamage = false; // 是否多段伤害
    private Area2D _area2D;

    public override void _Ready()
    {
        AnimatedSprite2D view = GodotTool.GetViewAndAutoPlay(this);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        ViewTool.View3In1(this, minScale, maxScale);
        _area2D = GetNode<Area2D>(NameConstants.AttackArea);
        if (_area2D != null)
        {
            _area2D.AreaEntered += OnArea2DAreaEntered;
            SetWorkingMode(false); // 初始不监测碰撞
        }
        _autoDieElapsed = 0f;
        _autoDieActive = true;
        if (IsPlayFlySound)
        {
            if (hurtType == EnumHurts.Cold)
            {
                SoundFxController.Instance?.PlayFx("Fx/icefly", "ice_fly", 4);
            }
        }
        //
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;
        //
        Damage = BulletConstants.GetDamage(objName);
        
        // RotationSpeed 随机一下正负
        if (RotationSpeed != 0)
        {
            RotationSpeed += (GD.Randf() * 20f) - 10f;
            if (GD.Randi() % 2 == 0) RotationSpeed = -RotationSpeed;
        }
    }

    // 用于Process计时的自动销毁
    private float _autoDieElapsed = 0f;
    private bool _autoDieActive = false;

    private bool _hasHit = false;
    private void OnArea2DAreaEntered(Area2D area)
    {
        if (!IsManyDamage && _hasHit) return; // 只处理第一个碰撞体
        bool isOk = DoTakeDamage(area);
        if (isOk) { _hasHit = true; __Die(); 
            var view = GetNodeOrNull<CanvasItem>(NameConstants.View);
            view.Visible = false;
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
            ViewTool.ViewZiFace(this);
            GameTool.RunnerBulletZeroWhenDieFx(this, delta, ref fadeElapsed, fadeDuration, fadeLowest);
        }
        else
        {
            ViewTool.View3In1(this, minScale, maxScale);
        }
        if (RotationSpeed != 0)
        {
            Rotation += Mathf.DegToRad(RotationSpeed) * (float)delta;
        }
    }
    [Export]
    public float RotationSpeed = 0f; // 每秒旋转角度
    [Export]
    public bool IsPlayFlySound = true;

    bool isDoDie = false;
    void DoDie() { if (!isDoDie) { isDoDie = true; __Die(); } }
    float maxY = GameContants.HorizonBulletY; // 子弹出界的y值
    float FixSpeedByY()
    {
        float cs = GameTool.FixBulletSpeedByY(SpeedInit, Position, maxY);
        if (cs <= (SpeedInit * 0.1f)) { DoDie(); }
        return cs;
    }

    public bool DoTakeDamage(Area2D area) => ObjTool.TakeDamage(area, objType, Damage, hurtType);
    public float Speed { get; set; } = BulletConstants.SpeedBasic; // 默认速度
    float SpeedInit = BulletConstants.SpeedBasic; // 初始速度
    public EnumHurts GetHurtType() => hurtType;
    public Vector2 Direction { get; set; } = Vector2.Up;

    public void SetDirection(Vector2 direction) => Direction = direction;
    public void FlipXDirection() => Direction = new Vector2(-Direction.X, Direction.Y);
    public void FlipYDirection() => Direction = new Vector2(Direction.X, -Direction.Y);

    async void __Die()
    {
        CloseArea();
        await ToSignal(GetTree().CreateTimer(fadeDuration), "timeout");
        QueueFree();
    }
    void CloseArea() { try { if (_area2D != null) _area2D.QueueFree(); } catch (Exception e) { } }

    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.Pea;
    [Export]
    public string objName = BulletConstants.BulletPeaColdName;
    public string GetObjName() => objName;

    public bool Init(string name = null) => true;
    public bool Die() => true;
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    private int Damage = BulletConstants.DamageBasic;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => 0;
    public Vector2 GetDirection() => Direction;
    public bool CanAttack() => true;

    public void SetWorkingMode(bool working)
    {
        if (working)
        {
            _area2D.Monitoring = true; // 监测碰撞
            // Speed = 0; SpeedInit = 0; // 冷冻子弹命中后速度归零
        }
        else
        {
            _area2D.Monitoring = false; // 不监测碰撞
        }
    }

    public bool IsWorking() => _area2D.Monitoring;
}
