using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZombiSAndExtraCopy : Node2D, IActionExtra, IStatus, IBeHurt, IInit, IEnmy
{
    AnimatedSprite2D view;
    AnimatedSprite2D viewExtra;
    AnimatedSprite2D iceFx;
    IStatus fatherStatus;

    public async void SwitchWalkLiveAnimation(int health, int healthInit) => AnimationTool.DoAniWalkHp(view, health, healthInit);
    public async void SwitchExtraLiveAnimation(int health, int healthInit)
    {
        float a = AnimationTool.DoAniExtraLiveHp(viewExtra, health, healthInit);
        if (a > 0f)
        {
            await ToSignal(GetTree().CreateTimer(a), "timeout");
            viewExtra.QueueFree();
        }
    }
    public void SwitchDieForBoomAnimation() => GodotTool.SwitchAnimatedSprite(this, NameConstants.DieForBoom);
    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        view.Visible = false;
        viewExtra = GetNodeOrNull<AnimatedSprite2D>(NameConstants.ViewExtra);
        iceFx = GetNodeOrNull<AnimatedSprite2D>(NameConstants.IceFreeze);
        fatherStatus = GetParent<IStatus>();
        // Init();

        // 尝试咆哮
        EnmyCenter.Instance?.PlayAlonePaoxiao(ObjName, this.GlobalPosition);
    }
    public void HideIceFx() => iceFx.Visible = false;
    public void ShowIceFx() => iceFx.Visible = true;
    public void SetColdFx(bool isCold)
    {
        if (view != null) ViewTool.SetLvJingBlue(view, 0.618f, isCold);
        if (viewExtra != null) ViewTool.SetLvJingBlue(viewExtra, 0.618f, isCold);
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
    public float HasDieAction() => 1f;
    public float HasInitAction() => 0.4f;

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
    public float GetMoveSpeedScale() => fatherStatus.GetMoveSpeedScale();
    public float GetAttackSpeedScale() => fatherStatus.GetAttackSpeedScale();
    public float GetAnimationSpeedScale() => fatherStatus.GetAnimationSpeedScale();
    //
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
        SwitchStatus(EnumEnmyStatus.Intro);
        view.Visible = true;
        return true;
    }

    int health = 10;
    int healthInit = 1000;
    int healthExtra = 10;
    int healthExtraInit = 1000;
    // 受伤控制（IHurt接口实现）
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (enumHurts != EnumHurts.Boom)
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
                CostHealthBody(yichu);
            }
        }
        else
        {
            CostHealthBody(damage + yichu);
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
                SwitchStatus(EnumEnmyStatus.Outro);
            }
        }
        return (health + healthExtra) > 0;
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

    async void DoBeHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, (healthExtra > 0) ? ExtraObj : BodyObj, this.GlobalPosition);
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.7f);
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }
    public Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts) => throw new NotImplementedException();
    public void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale) => throw new NotImplementedException();
    public void SetObjName(string name) => throw new NotImplementedException();

    [Export]
    public EnumEnmyStatus enmyStatus = EnumEnmyStatus.None;
    //
    public EnumEnmyStatus GetStatus() => enmyStatus;

    public void SwitchStatus(EnumEnmyStatus status)
    {
        switch (status)
        {
            case EnumEnmyStatus.Move:
                SwitchWalkLiveAnimation(health, healthInit);
                break;
            case EnumEnmyStatus.Attack:
                view.Play(AnimationConstants.AniAttack);
                break;
            case EnumEnmyStatus.Outro:
                AnimationTool.DoAniOutro(view);
                break;
            default:
                view.Play(AnimationConstants.AniDefault);
                break;
        }
        enmyStatus = status;
        if (status == EnumEnmyStatus.Move) (fatherStatus as IMove)?.StartMove();
        else (fatherStatus as IMove)?.PauseMove();
    }

    [Export]
    public EnumMoveType moveHasExtra = EnumMoveType.LineWalk;
    [Export]
    public EnumMoveType moveLoseExtra = EnumMoveType.LineWalk;
    public EnumMoveType GetEnumMoveType() {
        return (healthExtra > 0) ? moveHasExtra : moveLoseExtra;
    }

    public void SeeTarget(IObj obj)
    {
        throw new NotImplementedException();
    }

}
