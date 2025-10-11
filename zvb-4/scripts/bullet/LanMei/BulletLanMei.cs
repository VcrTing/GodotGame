using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletLanMei : Node2D, IObj, IBulletBase, IAttack, IWorking
{
    AnimatedSprite2D view;
    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        Init();
        view = GodotTool.FindNode2DByName(this, NameConstants.View) as AnimatedSprite2D;
    }
    async void __Die()
    {
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        QueueFree();
    }
    public bool Die()
    {
        __Die();
        return true;
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
    bool isDoDie = false;
    void DoDie()
    {
        if (!isDoDie)
        {
            Node2D uxArea = GetNodeOrNull<Node2D>(NameConstants.AttackArea);
            if (uxArea != null)
            {
                uxArea.QueueFree();
            }
            isDoDie = true; __Die();
        }
    }
    [Export]
    public bool isRotate = false;
    public void MoveBullet(Vector2 direction, float speed, double delta)
    {
        Position += direction * speed * (float)delta;
    }
    float __t = 0f;
    public override void _Process(double delta)
    {
        __t += (float)delta;
        if (__t > 8f) {  DoDie(); }

        Speed = FixSpeedByY();
        MoveBullet(Direction, Speed, delta);
        if (isDoDie)
        {
            ViewTool.ViewZiFace(this);
            // 死亡效果
            RunningDieFx(delta);
        }
        else
        {
            ViewTool.View3In1(this, minScale, maxScale);
        }

        if (isRotate)
        {
            Rotation += Mathf.DegToRad(60f) * (float)delta; // 每秒60度
        }
    }
    void RunningDieFx(double delta)
    {
        GameTool.RunnerBulletZeroWhenDieFx(this, view, delta, ref fadeElapsed, fadeDuration, fadeLowest);
    }
    // 渐隐相关变量
    float fadeElapsed = 0f;
    float fadeDuration = AnimationConstants.BulletFadeDieDuration;
    float fadeLowest = 0f;
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    private int Damage = 0;
    private int DamageExtra = 0;
    public int GetDamage() => Damage;
    public int GetDamageExtra() => DamageExtra;
    [Export]
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    string objName = BulletConstants.BulletLanMeiName;
    public string GetObjName() => objName;
    [Export]
    public EnumHurts hurtType { get; set; } = EnumHurts.Cold;
    public EnumHurts GetHurtType() => hurtType;
    public bool Init(string name = null)
    {        
        Damage = BulletConstants.GetDamage(objName);
        DamageExtra = BulletConstants.GetDamageExtra(objName);
        SpeedInit = GameTool.GetBulletInitialSpeed(objName);
        Speed = SpeedInit;
        return true;
    }
    public float Speed { get; set; } = 0; // 默认速度
    float SpeedInit = 0; // 初始速度
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上
    public Vector2 GetDirection() => Direction;
    public void SetDirection(Vector2 direction) => Direction = direction;
    public void FlipXDirection() => Direction = new Vector2(-Direction.X, Direction.Y);
    public void FlipYDirection() => Direction = new Vector2(Direction.X, -Direction.Y);
    //
    public void StartAttack()
    {
        Speed = 0;
        // 开启攻击范围
        IInit area = GetNodeOrNull<IInit>(NameConstants.AttackArea);
        if (area != null)
        {
            area.Init();
        }
        // 开启 溢出攻击范围
        IInit extraArea = GetNodeOrNull<IInit>(NameConstants.AttackExtraArea);
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
            Die();
        }
    }
    public bool IsWorking() => isWorkingMode;

    public bool CanAttack()
    {
        throw new NotImplementedException();
    }

}
