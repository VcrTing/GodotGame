
using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansSunFlower : Node2D, IWorking, IObj, IBeHurt
{

    public bool IsWorkingMode = false;

    public void SetWorkingMode(bool working)
    {
        IsWorkingMode = working;
    }
    public bool IsWorking() => IsWorkingMode;

    AnimatedSprite2D view;
    public override void _Ready()
    {
        view = GodotTool.GetViewAndAutoPlay(this);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        AdjustView();

        var scs = SunCenterSystem.Instance;
        if (scs != null)
        {
            SetWorkingMode(true);
        }
        _flowerWorking = GetNodeOrNull<FlowerWorkingReword>(NameConstants.Working);
        //
        
    }
    // 点击回调，销毁自身
    private void Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        AdjustView();
        if (IsWorkingMode)
        {
            _flowerWorking?.ProcessingForFlower(delta, this);
        }
    }

    // CCC

    FlowerWorkingReword _flowerWorking = null;
    
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;

    string objName = PlansConstants.SunFlower;
    public string GetObjName() => objName;

    public bool Init(string name = null) => true;


    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        return true;
    }

    // 受伤控制（IHurt接口实现）
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!canBeHurt) return false;
        bool died = ObjTool.RunningBeHurt(this, objType, damage, enumHurts);
        if (died)
        {
            _ = Die(objType, damage, enumHurts);
        }
        return died;
    }
    public bool BeCure(int cure)
    {
        GD.Print($"{Name} 治疗，回血 {cure}. ");
        return false;
    }
    bool canBeHurt = true;
    public void StopBeHurt() => canBeHurt = false;
    public void StartBeHurt() => canBeHurt = true;

    public bool Die() {
        // 解锁格子
        GeZi gz = GetParent() as GeZi;
        if (gz != null)
        {
            gz.UnLockGezi(this);
        }
        // 销毁自己
        QueueFree();
        return true;
    }
    public async Task<bool> Die(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        ObjTool.RunningDie(this, objType, damage, enumHurts);
        await ToSignal(GetTree().CreateTimer(AnimationConstants.GetDieAniTime(this)), "timeout");
        return Die();
    }

    public EnumMoveType GetEnumMoveType()
    {
        throw new NotImplementedException();
    }
}