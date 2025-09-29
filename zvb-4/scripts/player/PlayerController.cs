using Godot;
using ZVB4.Conf;


public partial class PlayerController : Node2D
{
    public static PlayerController Instance { get; private set; }
    public Vector2 _lastClickPosition;
    private Shooter _shooter;
    public Vector2 _shooterInitPosition;

    private bool _mouseHeld = false;
    private float _attackInterval = 0.2f;
    private const double LongPressThreshold = 0.3;
    private float _attackTimer = 0f;
    private double _mouseDownTime = 0f;
    private bool _isLongPressing = false;

    // Area2D attckArea;
    // RectangleShape2D attackRect;
    bool canAttack = true;

    public void SetCanAttack(bool value)
    {
        canAttack = value;
    }

    public override void _Ready()
    {
        Instance = this;
        AfterGetShooter();
        //
        // attckArea = GetNode<Area2D>(NameConstants.PlayerAttackArea);
        // var shapeNode = attckArea.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
        // attackRect = shapeNode?.Shape as RectangleShape2D;
    }

    public void TrashOldShooter()
    {
        try
        {
            Node2D nd = GodotTool.FindNode2DByName(this, NameConstants.Shooter);
            if (nd == null) return;
            nd.Name = NameConstants.Shooter + "_to_be_deleted";
            nd.QueueFree();
            _shooter = null;
        }
        catch
        {
        }
    }

    void AfterGetShooter()
    {
        try
        {
            Node2D nd = GodotTool.FindNode2DByName(this, NameConstants.Shooter);
            if (nd == null)
            {
                return;
            }
            if (nd is Shooter)
            {
                _shooter = nd as Shooter;
            }
            if (_shooter != null)
            {
                _shooterInitPosition = _shooter.GlobalPosition;
            }
        }
        catch
        {
        }
    }

    void Attack(Vector2 clickPosition)
    {
        if (_shooter == null)
        {
            AfterGetShooter();
        }
        // 必须要 _shooter
        if (_shooter == null) return;
        _shooterInitPosition = _shooter.GlobalPosition;
        // 计算方向
        Vector2 direction = (clickPosition - _shooterInitPosition).Normalized();
        // 让shooter朝向该方向旋转
        if (_shooter != null)
        {
            _shooter.RotateToDirection(direction);
            _shooter.AttackAtPosition(_shooterInitPosition, direction);
        }
    }

    bool __startAttack = false;
    Vector2 __lastClickPos = Vector2.Zero;
    public void TryAttack(Vector2 clickPosition)
    {
        __startAttack = true;
        __lastClickPos = clickPosition;
    }
    public void ReleaseAttack(Vector2 clickPosition)
    {
        __lastClickPos = clickPosition;
        __startAttack = false;
        firstAttack = false;
        __attackTime = 0;
        __attackSpeed = attackSpeedStart;
    }

    void Attack()
    {
        if (__startAttack) Attack(__lastClickPos);
    }

    public float bounceTime = 3f;

    public float touchShortPressTime = 0.05f; // 长按时间，秒

    float __attackTime = 0;
    bool firstAttack = false;
    public override void _Process(double delta)
    {
        // CCC
        if (__startAttack)
        {
            __attackTime += (float)delta;

            if (firstAttack)
            {
                if (__attackTime >= GetAttackSpeed(delta))
                {
                    __attackTime = 0;
                    Attack(__lastClickPos);
                }
            }
            else
            {
                if (__attackTime >= touchShortPressTime)
                {
                    firstAttack = true;
                    __attackTime = 0;
                    __attackSpeed = attackSpeedStart;
                    Attack(__lastClickPos);
                }
            }
        }
        else
        {

        }
    }

    public float __attackSpeed = 1f;
    public float attackSpeedStart = 1f;
    public float attackSpeedFast = 0.05f;
    public float attackSpeedStep = 0.02f;
    float GetAttackSpeed(double delta)
    {
        if (__attackSpeed > attackSpeedFast)
        {
            __attackSpeed -= attackSpeedStep;
            if (__attackSpeed < attackSpeedFast) __attackSpeed = attackSpeedFast;
        }
        return __attackSpeed;
    }
}
