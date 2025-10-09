using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZombiSAndExtra : Node2D, IActionExtra, IStatus, IBeHurt, IInit, IEnmy
{
    AnimatedSprite2D view;
    AnimatedSprite2D viewExtra;
    AnimatedSprite2D iceFx;
    //
    IStatus fatherStatus;

    public async void SwitchWalkLiveAnimation(int health, int healthInit)
    {
        float a = AnimationTool.DoAniWalkHp(view, health, healthInit);
    }
    public async void SwitchExtraLiveAnimation(int health, int healthInit)
    {
        float a = AnimationTool.DoAniExtraLiveHp(viewExtra, health, healthInit);
        if (a > 0f)
        {
            await ToSignal(GetTree().CreateTimer(a), "timeout");
            viewExtra.QueueFree();
        }
    }
    public void SwitchDieForBoomAnimation()
    {
        GodotTool.SwitchAnimatedSprite(this, NameConstants.DieForBoom);
    }

    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        viewExtra = GetNodeOrNull<AnimatedSprite2D>(NameConstants.ViewExtra);
        iceFx = GetNodeOrNull<AnimatedSprite2D>(NameConstants.IceFreeze);
        fatherStatus = GetParent<IStatus>();
        // Init();

        // 尝试咆哮
        EnmyCenter.Instance?.PlayAlonePaoxiao(ObjName, this.GlobalPosition);
    }
    public void HideIceFx()
    {
        iceFx.Visible = false;
    }
    public void ShowIceFx()
    {
        iceFx.Visible = true;
    }
    public void SetColdFx(bool isCold)
    {
        if (view != null)
        {
            ViewTool.SetLvJingBlue(view, 0.618f, isCold);
        }
        if (viewExtra != null)
        {
            ViewTool.SetLvJingBlue(viewExtra, 0.618f, isCold);
        }
    }
    // 
    public bool DoDieAction()
    {
        AnimationTool.DoAniOutro(view);
        return true;
    }
    public bool DoInitAction()
    {
        AnimationTool.DoAniIntro(view);
        return true;
    }
    public float HasDieAction()
    {
        return 1f;
    }
    public float HasInitAction()
    {
        return 0.4f;
    }

    // 通过调整播放速度来调整动画时间
    public void SetAnimationSpeedScale()
    {
        if (view != null)
        {
            view.SpeedScale = GetAnimationSpeedScale();
        }
    }
    public bool DoFreeze(float time)
    {
        SetAnimationSpeedScale();
        ShowIceFx();
        SetColdFx(true);
        return true;
    }
    public bool ReleaseFreeze()
    {
        SetAnimationSpeedScale();
        HideIceFx();
        return true;
    }
    public bool DoCold(float time)
    {
        SetAnimationSpeedScale();
        SetColdFx(true);
        return true;
    }
    public bool ReleaseCold()
    {
        SetAnimationSpeedScale();
        HideIceFx();
        SetColdFx(false);
        return true;
    }
    /**

    */
    public float GetMoveSpeedScale()
    {
        return fatherStatus.GetMoveSpeedScale();
    }
    public float GetAttackSpeedScale()
    {
        return fatherStatus.GetAttackSpeedScale();
    }
    public float GetAnimationSpeedScale()
    {
        return fatherStatus.GetAnimationSpeedScale();
    }

    int health = 10;
    int healthInit = 1000;
    int healthExtra = 10;
    int healthExtraInit = 1000;
    public bool Init(string objName = null)
    {
        ObjName = objName ?? ObjName;
        AnimationTool.DoAniDefault(view);
        healthInit = EnmyTypeConstans.GetHp(ObjName);
        health = healthInit;
        healthExtraInit = EnmyTypeConstans.GetExtraHp(ObjName);
        healthExtra = healthExtraInit;
        ReleaseCold();
        SwitchWalkLiveAnimation(health, healthInit);
        SwitchExtraLiveAnimation(healthExtra, healthExtraInit);
        return true;
    }

    // 受伤控制（IHurt接口实现）
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        // GD.Print($"{health} 受伤，扣血 extra {healthExtra}. ");
        if (enumHurts == EnumHurts.Boom)
        {

        }
        else
        {
            DoBeHurtEffect(objType, damage, enumHurts);
        }
        // 伤害计算
        int yichu = 0;
        if (healthExtra > 0)
        {
            yichu = CostHealthExtra(damage);
            if (yichu > 0)
            {
                int yc2 = CostHealthBody(yichu);
                if (yc2 > 0)
                {
                    yichu = yc2;
                }
                else
                {
                    yichu = 0;
                }
            }
        }
        else
        {
            yichu = CostHealthBody(damage + yichu);
        }

        // 死亡
        if (health <= 0)
        {
            if (enumHurts == EnumHurts.Boom)
            {
                SwitchDieForBoomAnimation();
            }
            else
            {
                AnimationTool.DoAniOutro(view);
            }
        }
        int res = (health + healthExtra);
        return res > 0;
    }
    int CostHealthExtra(int damage)
    {
        int yichu = 0;
        healthExtra -= damage;
        if (healthExtra < 0)
        {
            yichu = -healthExtra;
            healthExtra = 0;
        }
        SwitchExtraLiveAnimation(healthExtra, healthExtraInit);
        return yichu;
    }
    int CostHealthBody(int damage)
    {
        int yichu = 0;
        health -= damage;
        if (health < 0)
        {
            yichu = -health;
            health = 0;
        }
        SwitchWalkLiveAnimation(health, healthInit);
        return yichu;
    }
    [Export]
    public string ObjName { get; set; } = EnmyTypeConstans.ZombiS;
    [Export]
    public EnumWhatYouObj BodyObj { get; set; } = EnumWhatYouObj.ZombiSoftBody;
    [Export]
    public EnumWhatYouObj ExtraObj { get; set; } = EnumWhatYouObj.None;

    void DoBeHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (healthExtra > 0)
        {
            SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, ExtraObj, this.GlobalPosition);
        }
        else
        {
            // GD.Print("僵尸本体受伤 BodyObj =" + BodyObj + " enumHurts =" + this.GlobalPosition);
            SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, BodyObj, this.GlobalPosition);
        }
        HurtFx();
    }
    async void HurtFx()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.7f);
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }
    public Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        throw new NotImplementedException();
    }

    public void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale)
    {
        throw new NotImplementedException();
    }

    public void SetObjName(string name)
    {
        throw new NotImplementedException();
    }

    [Export]
    public EnumEnmyStatus enmyStatus = EnumEnmyStatus.None;

    public EnumEnmyStatus GetStatus() => enmyStatus;

    public void SwitchStatus(EnumEnmyStatus status)
    {
        switch (status)
        {
            case EnumEnmyStatus.None:
                view.Play(AnimationConstants.AniDefault);
                break;
            case EnumEnmyStatus.Idle:
                view.Play(AnimationConstants.AniDefault);
                break;
            case EnumEnmyStatus.Move:
                SwitchWalkLiveAnimation(health, healthInit);
                break;
            case EnumEnmyStatus.Attack:
                view.Play(AnimationConstants.AniAttack);
                break;
            case EnumEnmyStatus.Working:
                SwitchWalkLiveAnimation(health, healthInit);
                break;
            default:
                break;
        }
        enmyStatus = status;
    }

    [Export]
    public EnumMoveType moveType = EnumMoveType.LineWalk;
    public EnumMoveType GetEnumMoveType() => moveType;
}
