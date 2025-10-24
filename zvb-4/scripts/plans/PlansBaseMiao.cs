using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class PlansBaseMiao : Node2D, IWorking, IObj, IAttack
{
    [Export]
    public string PlanName { get; set; } = PlansConstants.Pea;
    [Export]
    public bool IsWorkingMode = false;
    public void SetWorkingMode(bool working)
    {
        IsWorkingMode = working;
    }
    EnumObjType objType = EnumObjType.System;
    public EnumObjType GetEnumObjType()
    {
        return objType;
    }
    public string GetObjName()
    {
        return PlanName;
    }
    public int GetDamage()
    {
        return 0;
    }
    public int GetDamageExtra()
    {
        return 0;
    }

    public float GenerateTime = 10.0f; // 生长时间，秒

    bool IsComplete = false;

    private AnimatedSprite2D _viewAnim;
    private Area2D _area2D;

    Vector2 _initPosition = Vector2.Zero;

    public override void _Ready()
    {

    }

    public bool Init(string plansName)
    {
        SetWorkingMode(true);
        _initPosition = Position;
        PlanName = plansName;
        // 通过PlanName获取生长时间
        GenerateTime = PlansConstants.GetPlanGrowTime(PlanName);
        // 获取动画节点View
        _viewAnim = GetNodeOrNull<AnimatedSprite2D>(PlansConstants.Miao);
        if (_viewAnim != null)
        {
            _viewAnim.Play(NameConstants.Default);
        }
        _area2D = GetNodeOrNull<Area2D>(NameConstants.MiaoArea);
        if (_area2D != null)
        {
            _area2D.Connect("input_event", new Callable(this, nameof(OnAreaInputEvent)));
        }
        // 10秒后生长完成
        StartGrow();
        return true;
    }

    float __gt = 0;
    float __generateTime = 0;
    public async void StartGrow()
    {
        var rand = new Random();
        float generateTime = GenerateTime + (float)(rand.NextDouble() * 2.0f);
        __generateTime = generateTime;
        __gt += 0.0001f;
    }

    public override void _Process(double delta)
    {
        if (__gt > 0) {
            __gt += (float)delta;
            if (__gt >= __generateTime) {
                __gt = 0;
                OnGrowFinished();
            }
        }
    }

    bool _isInAttackArea = false;
    // 回到初始位置
    public void BackToPosition()
    {
        ChangeShooterAttackStatus(true); // 允许射手攻击
        Position = _initPosition;
    }
    // 种下后，解锁花盆占用
    public string ReleasePlanting()
    {
        ChangeShooterAttackStatus(true); // 允许射手攻击
        FlowerPeng flowerPeng = GetParent<FlowerPeng>();
        if (flowerPeng != null)
        {
            flowerPeng.ReleaseLock();
            QueueFree();
        }
        return PlanName;
    }

    private bool _isDragging = false;

    // 停止射手攻击
    void ChangeShooterAttackStatus(bool can = true)
    {
        var ps = PlayerController.Instance;
        if (ps != null)
        {
            if (can)
            {
                ps.UnLockAttack();
            }
            else
            {
                ps.ForbiddenAttack();
            }
        }
    }

    private void OnAreaInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (!IsComplete) return; // 只有生长完成后才能拖动
        if (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Left)
        {
            _isDragging = true;
            ChangeShooterAttackStatus(false); // 停止射手攻击
        }
    }

    async void OnTouchRealease()
    {
        bool succ = false;
        // 是否放进了射手工作台
        ShooterWorkTable swt = ShooterWorkTable.Instance;
        if (swt != null && swt.IsInMe(GetGlobalMousePosition()))
        {
            succ = swt.HandleCollision(PlanName);
            if (succ)
            {
                ReleasePlanting();
            }
        }
        // 否
        _isDragging = false;
        if (!succ)
        {
            BackToPosition();
        }
        // 延迟 0.1f
    }
    public override void _Input(InputEvent @event)
    {
        if (_isDragging)
        {
            if (@event is InputEventMouseButton mouse && !mouse.Pressed)
            {
                OnTouchRealease();
            }
            else if (@event is InputEventMouseMotion motion)
            {
                // 鼠标移动，节点跟随（父节点坐标整合）
                var globalPos = GetGlobalMousePosition();
                if (GetParent() is Node2D parent2d)
                {
                    Position = parent2d.ToLocal(globalPos);
                }
                else
                {
                    Position = globalPos;
                }
            }
        }
    }

    List<string> InitWorkPlans = new List<string>
    {
        PlansConstants.SunFlower, PlansConstants.RewordFlower, PlansConstants.SunGu
    };

    // 生长完成方法
    protected virtual void OnGrowFinished()
    {
        IsComplete = true;
        // 删除Miao动画节点
        var miaoNode = GetNodeOrNull(new NodePath(PlansConstants.Miao));
        // 不再在进入时直接拖动
        {
            miaoNode.QueueFree();
        }
        _area2D.Visible = true;
        Node2D plan = PlansConstants.GeneratePlans(this, PlanName);
        if (plan != null) {
            if (InitWorkPlans.Contains(PlanName)) {
                IWorking iw = plan as IWorking;
                iw.SetWorkingMode(true);
            }
        }
        // 播放生长完成音效
        SoundFxController.Instance?.PlayFx("Ux/grow", "grow", 4);
    }

    public bool CanDie()
    {
        return true;
    }

    public bool Die()
    {
        QueueFree();
        return true;
    }

    public bool IsWorking() => IsWorkingMode;

    public bool CanAttack()
    {
        throw new NotImplementedException();
    }

}
