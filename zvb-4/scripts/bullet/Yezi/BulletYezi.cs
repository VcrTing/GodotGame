using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletYezi : Node2D, IBulletBase, IObj, IAttack, IWorking
{
    string objName = BulletConstants.BulletYeziName;
    private Area2D _attackAreaExtra;
    private Area2D _attackArea;
    private AnimatedSprite2D _view;
    //
    public override void _Ready()
	{
		_attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
		if (_attackArea != null)
		{
			_attackArea.AreaEntered += OnAttackAreaEntered;
		}
		_attackAreaExtra = GetNodeOrNull<Area2D>(NameConstants.AttackExtraArea);
        if (_attackAreaExtra != null)
        {
            _attackAreaExtra.AreaEntered += OnAttackAreaEnteredExtra;
        }
        _view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        Init();
	}
    public bool Init(string name = null)
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        ViewTool.View3In1(this, minScale, maxScale);
        _autoDieElapsed = 0f;
        _autoDieActive = true;
        //
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;
        Damage = BulletConstants.GetDamage(objName);
        DamageExtra = BulletConstants.GetDamageExtra(objName);
        //
        SetWorkingMode(false);
        return true;
    }
    private bool _isDead = false;
    bool groupHit = false;
	private void OnAttackAreaEntered(Area2D area)
	{
        if (_isDead) return; float d = Damage;
        if (groupHit) { d = Damage / 2f; }
        else { _view.Visible = false; Speed = 0f; SpeedInit = 0f; }
        groupHit = true; DoTakeDamage(area, d);
	}
    // 伤害处理方法
    public bool DoTakeDamage(Area2D area, float damage) => ObjTool.TakeDamage(area, GetEnumObjType(), (int)damage, hurtType);
	private void OnAttackAreaEnteredExtra(Area2D area) {
        if (_isDead) return;
        DoTakeDamage(area, DamageExtra);
    }
    public float Speed { get; set; } = BulletConstants.SpeedBasic; // 默认速度
    float SpeedInit = BulletConstants.SpeedBasic; // 初始速度
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.Vip;
    public EnumHurts GetHurtType() => hurtType;
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上
    public Vector2 GetDirection() => Direction;
    public void SetDirection(Vector2 direction)
    {
        Direction = direction;
        UpdateRotationByDirection();
    }
    public void FlipXDirection() {
        Direction = new Vector2(-Direction.X, Direction.Y);
        UpdateRotationByDirection();
    }
    public void FlipYDirection() {
        Direction = new Vector2(Direction.X, -Direction.Y);
        UpdateRotationByDirection();
    }
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    public string GetObjName() => objName;
    // 初始化子弹
    private void UpdateRotationByDirection()
    {
        if (Direction.LengthSquared() > 0.0001f)
            Rotation = Vector2.Up.AngleTo(Direction.Normalized());
    }
    // 是否可以攻击
    public bool CanAttack() => true;
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool Die() => throw new NotImplementedException();
    private int Damage = BulletConstants.DamageBasic;
    int DamageExtra = 0;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => DamageExtra;

    bool isWorking = true;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        _attackArea.Monitoring = working;
        _attackArea.Monitorable = working;
        _attackAreaExtra.Monitoring = working;
        _attackAreaExtra.Monitorable = working;
    }
    public bool IsWorking() => isWorking;

    private float _autoDieElapsed = 0f;
    private bool _autoDieActive = false;
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
        // 给view进行旋转
        _view.Rotation += Mathf.DegToRad(RotationSpeed) * (float)delta; // 每秒360度
    }
    [Export]
    public float RotationSpeed { get; set; } = 520f; // 旋转速度，度/秒
    bool isDoDie = false;
    void DoDie() { if (!isDoDie) { isDoDie = true; __Die(); } }
    async void __Die()
    {
        SetWorkingMode(false);
        await ToSignal(GetTree().CreateTimer(fadeDuration), "timeout");
        QueueFree();
    }
    float maxY = GameContants.HorizonBulletY; // 子弹出界的y值
    float FixSpeedByY()
    {
        float cs = GameTool.FixBulletSpeedByY(SpeedInit, Position, maxY);
        if (cs <= (SpeedInit * 0.1f)) { DoDie(); }
        return cs;
    }
    public void MoveBullet(Vector2 direction, float speed, double delta)
    {
        Position += direction * speed * (float)delta;
    }

}
