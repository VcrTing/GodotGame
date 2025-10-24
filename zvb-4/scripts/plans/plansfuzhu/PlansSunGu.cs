using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansSunGu : Node2D, IWorking, IObj, IBeHurt
{
    float growTime = PlansConstants.SunPlansGrowTime;
    string objName = PlansConstants.SunGu;
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    public string GetObjName() => objName;
    public bool IsWorkingMode = false;
    public void SetWorkingMode(bool working) => IsWorkingMode = working;
    public bool IsWorking() => IsWorkingMode;
    AnimatedSprite2D view;
    public override void _Ready()
    {
        view = GodotTool.GetViewAndAutoPlay(this);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        _flowerWorking = GetNodeOrNull<FlowerWorkingReword>(NameConstants.Working);
    }

    float smallScale = 0f;
    void DoGrowing(double delta)
    {
        if (__t <= 0f) return;
        float v = __t - growTime;
        float s = (PlansConstants.BiggerRateMaxGu - 1) * v;
        s += 1;
        s = (float)Math.Sqrt(s);
        if (s >= PlansConstants.BiggerRateMaxGu)
        {
            s = PlansConstants.BiggerRateMaxGu;
        }
        view.Position = new Vector2(view.Position.X, view.Position.Y - (s/9));
        //
        maxScale = smallScale * s;
        minScale = ViewTool.GetYouMinScale(maxScale);
        if (s >= PlansConstants.BiggerRateMaxGu)
        {
            __t = -100f;
        }
    }
    bool isStartGrow = false;
    void DoGrowingStart()
    {
        if (isStartGrow) return; isStartGrow = true;
        smallScale = maxScale;
    }

    float __t = 0.0001f;
    public override void _Process(double delta)
    {
        ViewTool.View3In1(this, minScale, maxScale);
        if (IsWorkingMode)
        {
            _flowerWorking?.ProcessingForFlower(delta, this);
        }
        if (__t > 0f)
        {
            __t += (float)delta;
            if (__t >= growTime)
            {
                DoGrowingStart();
                DoGrowing(delta);
            }
        }
    }
    FlowerWorkingReword _flowerWorking = null;
    public bool Init(string name = null) => true;
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    // 受伤控制（IHurt接口实现）
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        bool died = ObjTool.RunningBeHurt(this, objType, damage, enumHurts);
        if (died)
        {
            _ = Die(objType, damage, enumHurts);
        }
        return died;
    }
    public bool Die() {
        GeZi gz = GetParent() as GeZi;
        if (gz != null) gz.UnLockGezi(this);
        QueueFree();
        return true;
    }
    public async Task<bool> Die(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        ObjTool.RunningDie(this, objType, damage, enumHurts);
        await ToSignal(GetTree().CreateTimer(AnimationConstants.GetDieAniTime(this)), "timeout");
        return Die();
    }
    public EnumMoveType GetEnumMoveType() => throw new NotImplementedException();
    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts) => ObjTool.DoPlansCure(view as IHealth, objType, cureAmount);

}
