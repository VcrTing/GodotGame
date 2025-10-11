using Godot;
using ZVB4.Conf;
using ZVB4.Entity;


public partial class PlayerController : Node2D
{
    public static PlayerController Instance { get; private set; }
    public Vector2 _lastClickPosition;
    private ShooterWrapper _shooter;
    public Vector2 _shooterInitPosition;
    public override void _Ready()
    {
        Instance = this;
        AfterGetShooter();
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
            if (nd is ShooterWrapper) { _shooter = nd as ShooterWrapper; }
            if (_shooter != null) { _shooterInitPosition = _shooter.GlobalPosition; }
        }
        catch { }
    }

    bool canAttack = true;
    public void ForbiddenAttack()
    {
        canAttack = false;
        ReleaseAttackClear(__lastClickPos, true);
    }
    public void ReleaseAttack()
    {
        canAttack = true;
    }
    //
    void Attack(Vector2 clickPosition, bool isFirstAttack)
    {
        // GD.Print("攻击 canAttack = " + canAttack);
        if (canAttack == false) return;
        // 确保有 _shooter
        if (_shooter == null) AfterGetShooter();
        // 必须要 _shooter
        if (_shooter == null) return;
        _shooter.Attack(clickPosition, isFirstAttack);
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
    public override void _Process(double delta)
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
