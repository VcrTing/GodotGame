using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletPeaGold : Node2D, IBulletBase, IObj, IAttack
{
	[Export]
	public int MaxHits = 20; // 攻击 xx 个人就伤害递减
	[Export]
	public float MinScale = 0.4f;
	[Export]
	public float MinAlpha = 0.382f;
	private float _damage;
	private int _hitCount = 0;
	private bool _isDead = false;
    public float Speed { get; set; } = BulletConstants.SpeedBasic; // 默认速度
    float SpeedInit = BulletConstants.SpeedBasic; // 初始速度
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.Vip;
    public EnumHurts GetHurtType() => hurtType;
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上
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
    string objName = BulletConstants.BulletPeaGoldName;
    public string GetObjName() => objName;
    public bool Init(string name = null)
    {
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;
        Damage = BulletConstants.GetDamage(objName);
        _damage = Damage;
        UpdateVisuals();
        // KKK
        // 根据方向旋转自身，默认方向为向上
        UpdateRotationByDirection();
        // 启动计时器变量，Process里计时
        _autoDieElapsed = 0f;
        _autoDieActive = true;
        return true;
    }

    private void UpdateRotationByDirection()
    {
        // 默认方向为Vector2.Up (0, -1)
        if (Direction.LengthSquared() > 0.0001f)
        {
            // Rotation = Vector2.Up.AngleTo(Direction);
        }
    }
    public bool CanAttack() => true;
    private int Damage = BulletConstants.DamageBasic;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => 0;
    public Vector2 GetDirection() => Direction;
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
        Init();
	}
    EnumHurts enumHurts = EnumHurts.Vip;
    // 用于Process计时的自动销毁
    private float _autoDieElapsed = 0f;
    private bool _autoDieActive = false;
    bool isDoDie = false;
    async void __Die()
    {
        await ToSignal(GetTree().CreateTimer(0.4f), "timeout");
        QueueFree();
    }
    void DoDie()
    {
        if (!isDoDie)
        {
            isDoDie = true;
            __Die();
        }
    }
    float maxY = GameContants.HorizonBulletY;
    float FixSpeedByY()
    {
        float cs = GameTool.FixBulletSpeedByY(SpeedInit, Position, maxY);
        if (cs <= (SpeedInit * 0.1f))
        {
            DoDie();
        }
        return cs;
    }
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
            // 死亡效果
            RunningDieFx(delta);
        }
        else
        {
            AdjustView();
        }
    }
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        // GD.Print($"BulletPeaGold AdjustView Position.Y={Position.Y}, Scale={Scale}");
        return true;
    }
    // 渐隐相关变量
    float fadeElapsed = 0f;
    float fadeDuration = AnimationConstants.BulletFadeDieDuration;
    float fadeLowest = 0f;
    void RunningDieFx(double delta)
    {
        GameTool.RunnerBulletZeroWhenDieFx(this, delta, ref fadeElapsed, fadeDuration, fadeLowest);
    }
    public void MoveBullet(Vector2 direction, float speed, double delta)
    {
        // 移动始终用世界坐标方向，与节点旋转无关
        Position += direction.Normalized() * speed * (float)delta;
    }
    public bool Die()
    {
        throw new NotImplementedException();
    }


    private void UpdateVisuals()
    {
        float t = Mathf.Clamp((float)_hitCount / MaxHits, 0f, 1f);
        // float scale = Mathf.Lerp(1f, MinScale, t);
        float alpha = Mathf.Lerp(1f, MinAlpha, t);
        // Scale = new Vector2(scale, scale);
        var modulate = Modulate;
        modulate.A = alpha;
        Modulate = modulate;
    }
    private Area2D _attackArea;
    
	private Area2D _attackAreaExtra;
	private void OnAttackAreaEntered(Area2D area)
	{
        if (_isDead) return;
        // GD.Print($"BulletPeaGold OnAttackAreaEntered _hitCount={_hitCount}, _damage={_damage}");
        //
        DoTakeDamage(area, _damage);
		_hitCount++;
		// 伤害递减
		float t = Mathf.Clamp((float)_hitCount / MaxHits, 0f, 1f);
        _damage = Mathf.Lerp(Damage, 0f, t);
		// 
		UpdateVisuals();
		if (_damage <= 0f || _hitCount >= MaxHits)
		{
			_isDead = true;
			QueueFree();
		}
	}
    // 伤害处理方法
    void DoTakeDamage(Area2D area, float damage)
    {
        if (area is IHurtBase hurt)
        {
            hurt.TakeDamage(objType, (int)damage, enumHurts);
        }
        else
        {
            var parent = area.GetParent();
            if (parent is IHurtBase hurtParent)
            {
                hurtParent.TakeDamage(objType, (int)damage, enumHurts);
            }
        }
    }
    
	private void OnAttackAreaEnteredExtra(Area2D area) {
        if (_isDead) return;
        float _d = _damage / 3;
        GD.Print($"BulletPeaGold OnAttackAreaEnteredExtra _hitCount={_hitCount}, _damage={_damage}, _d={_d}");
        DoTakeDamage(area, _d);
    }
}
