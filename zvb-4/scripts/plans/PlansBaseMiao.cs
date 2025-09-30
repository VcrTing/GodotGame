using Godot;
using System;
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

    public override async void _Ready()
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
        // 获取Area2D并监听input_event
        _area2D = GetNodeOrNull<Area2D>(NameConstants.MiaoArea);
        if (_area2D != null)
        {
            _area2D.Connect("input_event", new Callable(this, nameof(OnAreaInputEvent)));
            // _area2D.BodyEntered += OnBodyEntered;
            // _area2D.BodyExited += OnBodyExited;
        }
        // 10秒后生长完成
        StartGrow();
        return true;
    }

    public async void StartGrow()
    {
        var rand = new Random();
        float generateTime = GenerateTime + (float)(rand.NextDouble() * 2.0f);
        await ToSignal(GetTree().CreateTimer(generateTime), "timeout");
        OnGrowFinished();
    }

    bool _isInAttackArea = false;


    void BackToPosition()
    {
        Position = _initPosition;
    }

    private bool _isDragging = false;

    private void OnAreaInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (!IsComplete) return; // 只有生长完成后才能拖动
        if (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Left)
        {
            _isDragging = true;
        }
    }

    async void OnRealease()
    {
        // 
        bool succ = false;
        // 是否放进了射手工作台
        ShooterWorkTable swt = ShooterWorkTable.Instance;
        if (swt != null && swt.IsInMe(GetGlobalMousePosition()))
        {
            succ = swt.HandleCollision(PlanName);
            if (succ)
            {
                ZhongXia();
            }
        }
        // 是否放在攻击区

        //
        _isDragging = false;
        if (!succ)
        {
            BackToPosition();
        }
        // 延迟0.1f
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        PlayerController.Instance?.SetCanAttack(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (_isDragging)
        {
            if (@event is InputEventMouseButton mouse && !mouse.Pressed)
            {
                OnRealease();
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

                PlayerController.Instance?.SetCanAttack(false);
            }
        }
    }


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
        //
        _area2D.Visible = true;
        // 
        ViewPlans();

        // 播放生长完成音效
        SoundFxController.Instance?.PlayFx("Ux/grow", "grow", 4);
    }

    void ViewPlans()
    {
        // 根据PlanName获取场景路径
        string scenePath = PlansConstants.GetPlanScene(PlanName);
        if (!string.IsNullOrEmpty(scenePath))
        {
            var planScene = GD.Load<PackedScene>(scenePath);
            if (planScene != null)
            {
                var planInstance = planScene.Instantiate();
                IObj obj = planInstance as IObj;
                if (obj != null)
                {
                    obj.Init(PlanName);
                }
                Node2D node2D = planInstance as Node2D;
                float sc = node2D.Scale.X;
                AddChild(planInstance);
            }
        }
    }

    // 种下后，解锁花盆占用
    public string ZhongXia()
    {
        FlowerPeng flowerPeng = GetParent<FlowerPeng>();
        if (flowerPeng != null)
        {
            flowerPeng.ReleaseLock();
            QueueFree();
        }
        return PlanName;
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

    public bool AdjustView()
    {
        throw new NotImplementedException();
    }

}
