using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansQieZi : Node2D, IObj, IBeHurt, IWorking
{
    AnimatedSprite2D view;

    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.QieZi;
    public string GetObjName() => objName;

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool Init(string name = null)
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        return true;
    }

    public async void SwitchLiveAnimation(int health, int healthInit)
    {
        float a = AnimationTool.DoAniExtraLiveHp(view, health, healthInit);
        if (a > 0f)
        {
            await ToSignal(GetTree().CreateTimer(a), "timeout");
        }
    }
    float initScale = 1f;
    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        initScale = Scale.X;
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
    }

    bool canBeHurt = true;
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!canBeHurt) return false;
        IHealth ih = view as IHealth;
        if (ih == null) return false;
        int yichu = ih.CostHealth(objType, damage, enumHurts);
        if (yichu > 0)
        {
            Die(objType, yichu, enumHurts); return false;
        }
        return true;
    }
    float outroTime = 0.5f;
    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        // 删掉BeHurtArea
        Node2D beHurtArea = GetNodeOrNull<Node2D>(NameConstants.BeHurtArea);
        if (beHurtArea != null)
        {
            beHurtArea.QueueFree();
        }
        // 销毁自己
        await ToSignal(GetTree().CreateTimer(outroTime), "timeout");
        return Die();
    }
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

    public override void _Process(double delta)
    {
        ViewTool.View3In1(this, minScale, maxScale);
        if (__t > 0f)
        {
            __t += (float)delta;
            if (__t > 2f)
            {
                EndChange();
                __t = 0f;
            }
        }
    }

    bool isWorking = false;

    string rewordPlans = "";
    float __t = 0f;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        if (working)
        {
            StartChange();
        }
    }

    void StartChange()
    {
        rewordPlans = RewordConstants.GetQieZiRewordPlans();
        __t = 0.0001f;
    }
    void EndChange()
    {
        PlansBaseMiao miao = GetParent<PlansBaseMiao>();
        if (miao != null)
        {
            miao.SwitchPlans(rewordPlans);
        }
    }

    public bool IsWorking() => isWorking;

    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts) => ObjTool.DoPlansCure(view as IHealth, objType, cureAmount);

}
