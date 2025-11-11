using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZombiSAndExtra : Node2D, ICcActionExtra, IStatus, IBeHurt, IInit, IEnmy
{
    [Export]
    public string ObjName { get; set; } = EnmyTypeConstans.ZombiS;
    [Export]
    public EnumWhatYouObj BodyObj { get; set; } = EnumWhatYouObj.ZombiSoftBody;
    [Export]
    public EnumWhatYouObj ExtraObj { get; set; } = EnumWhatYouObj.None;
    public EnumWhatYouObj GetWhatYouObj() => ExtraObj;

    AnimatedSprite2D view;
    AnimatedSprite2D viewChanged;
    AnimatedSprite2D viewExtra;
    AnimatedSprite2D iceFx;
    
    public async void SwitchWalkLiveAnimation(int health, int healthInit) => AnimationTool.DoAniWalkHp(IsChanged ? viewChanged : view, health, healthInit);
    public async void SwitchExtraLiveAnimation(int health, int healthInit)
    {
        float a = AnimationTool.DoAniExtraLiveHp(viewExtra, health, healthInit);
        if (a > 0f)
        {
            await ToSignal(GetTree().CreateTimer(a), "timeout");
            if (viewExtra != null) viewExtra.Visible = false;
        }
    }
    public void SwitchDieForBoomAnimation()
    {
        __changeTime = 0f; (fatherNode as IMove)?.PauseMove();
        AnimationTool.DoAniDieForBoom(IsChanged ? viewChanged : view);
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
        // 穿透
        bool isonlyhurtbody = ZombiTool.IsOnlyHurtBody(ExtraObj, enumHurts);

        // 伤害计算
        int yichu = 0;
        if (healthExtra <= 0 || isonlyhurtbody)
        {
            CostHealthBody(damage + yichu);
        }
        else
        {
            yichu = CostHealthExtra(damage);
            if (yichu > 0)
            {
                CostHealthBody(yichu);
            }
        }

        // 死亡
        if (health <= 0)
        {
            if (enumHurts == EnumHurts.Boom)
            {
                SwitchDieForBoomAnimation();
            }
        }
        float res = (health + healthExtra);
        if (res < (healthInit + healthExtraInit) * 0.5f)
        {
            RunningJudgeChanging(EnumWhenChangingType.HealthBelowHalf);
        }

        if (health <= 0) return false;
        return res > 0;
    }
    int CostHealthExtra(int damage)
    {
        int yichu = 0;
        healthExtra -= damage;
        if (healthExtra <= 0)
        {
            yichu = -healthExtra;
            healthExtra = 0;
            RunningJudgeChanging(EnumWhenChangingType.LoseViewExtra);
            ExtraObj = EnumWhatYouObj.None;
        }
        SwitchExtraLiveAnimation(healthExtra, healthExtraInit);
        return yichu;
    }
    int CostHealthBody(int damage)
    {
        int yichu = 0;
        health -= damage;
        if (health <= 0)
        {
            yichu = -health;
            health = 0;
            if (viewExtra != null) viewExtra.Visible = false;
        }
        SwitchWalkLiveAnimation(health, healthInit);
        return yichu;
    }
    async void DoBeHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, (healthExtra > 0) ? ExtraObj : BodyObj, this.GlobalPosition);
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.7f);
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }

    public Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts) => throw new NotImplementedException();

    // 通过调整播放速度来调整动画时间
    public void HideIceFx() => iceFx.Visible = false;
    public void ShowIceFx() => iceFx.Visible = true;    public void SetColdFx(bool isCold)
    {
        if (view != null) ViewTool.SetLvJingBlue(IsChanged ? viewChanged : view, 0.5f, isCold);
        if (viewExtra != null) ViewTool.SetLvJingBlue(viewExtra, 0.5f, isCold);
    }
    public void SetAnimationSpeedScale()
    {
        if (view != null)
        {
            view.SpeedScale = GetAnimationSpeedScale();
            viewChanged.SpeedScale = GetAnimationSpeedScale();
            try
            {
                if (viewExtra != null) viewExtra.SpeedScale = GetAnimationSpeedScale();
            }
            catch (Exception e) { }
        }
    }
    public bool DoFreeze(float time)
    {
        SetAnimationSpeedScale();
        ShowIceFx();
        SetColdFx(true);
        return true;
    }
    public bool DoCold(float time)
    {
        SetAnimationSpeedScale();
        SetColdFx(true);
        return true;
    }
    public bool ReleaseFreeze()
    {
        SetAnimationSpeedScale();
        HideIceFx();
        return true;
    }
    public bool ReleaseCold()
    {
        SetAnimationSpeedScale();
        HideIceFx();
        SetColdFx(false);
        return true;
    }
    public float GetMoveSpeedScale() => (fatherNode as IStatus)?.GetMoveSpeedScale() ?? 1f;
    public float GetAttackSpeedScale() => (fatherNode as IStatus)?.GetAttackSpeedScale() ?? 1f;
    public float GetAnimationSpeedScale() {
        if (health <= 0) { return (fatherNode as IStatus)?.GetAnimationSpeedScaleNoCold() ?? 1f; }
        return (fatherNode as IStatus)?.GetAnimationSpeedScale() ?? 1f;
    }

    Node2D redEye;
    public void StartRedMode() => redEye.Visible = true;
    public void EndRedMode() => redEye.Visible = false;
    public void JudgeOpenRedEyeMode(float redeyeratio) => throw new NotImplementedException();

    Node2D fatherNode;
    public bool Init(string objName = null)
    {
        ObjName = objName ?? ObjName;
        AnimationTool.DoAniDefault(view);
        healthInit = EnmyTypeConstans.GetHp(ObjName);
        health = healthInit;
        healthExtraInit = EnmyTypeConstans.GetExtraHp(ObjName);
        healthExtra = healthExtraInit;
        SwitchWalkLiveAnimation(health, healthInit);
        SwitchExtraLiveAnimation(healthExtra, healthExtraInit);
        viewChanged.Visible = false;
        view.Visible = true;
        fatherNode = GetParent<Node2D>();

        return true;
    }
    public void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale) => throw new NotImplementedException();
    public void SetObjName(string name) => ObjName = name;

    EnumEnmyStatus enmyStatus = EnumEnmyStatus.None;
    public EnumEnmyStatus GetStatus() => enmyStatus;

    public void SwitchStatus(EnumEnmyStatus status)
    {
        // GD.Print($"ZombiSAndExtra.SwitchStatus: {status}, changing={IsChanged}");
        enmyStatus = status;
        switch (status)
        {
            case EnumEnmyStatus.Move:
                SwitchWalkLiveAnimation(health, healthInit);
                break;
            case EnumEnmyStatus.Attack:
                AnimationTool.DoAniAttack(IsChanged ? viewChanged : view);
                break;
            case EnumEnmyStatus.Changing:
                AnimationTool.DoAniChanging(IsChanged ? viewChanged : view);
                break;
            case EnumEnmyStatus.Outro:
                AnimationTool.DoAniOutro(IsChanged ? viewChanged : view);
                break;
            case EnumEnmyStatus.None:
            default:
                AnimationTool.DoAniDefault(IsChanged ? viewChanged : view);
                break;
        }
        enmyStatus = status;
        if (status == EnumEnmyStatus.Move || status == EnumEnmyStatus.Changing || status == EnumEnmyStatus.Attack) (fatherNode as IMove)?.StartMove();
        else (fatherNode as IMove)?.PauseMove();
        //
        SetAnimationSpeedScale();
    }
    [Export]
    public EnumMoveType MoveWhenAttack = EnumMoveType.Stone;
    [Export]
    public EnumMoveType MoveChangeBefore = EnumMoveType.LineWalk;
    [Export]
    public EnumMoveType MoveChanging = EnumMoveType.LineWalk;
    [Export]
    public EnumMoveType MoveChangeAfter = EnumMoveType.LineWalk;
    public EnumMoveType GetEnumMoveType()
    {
        if (enmyStatus == EnumEnmyStatus.Attack) return MoveWhenAttack;
        if (enmyStatus == EnumEnmyStatus.Changing) return MoveChanging;
        if (IsChanged) return MoveChangeAfter;
        return MoveChangeBefore;
    }
    [Export]
    public float ChangingTime = 2f;
    public float HasChangeAction() => ChangingTime;
    public bool StartChangeAction()
    {
        SwitchStatus(EnumEnmyStatus.Changing); __changeTime = 0.0001f; StartChange(); return true;
    }
    public bool EndChangeAction()
    {
        if (health <= 0) return false;
        view.Visible = false; viewChanged.Visible = true; IsChanged = true;
        if (IsInAttack() > 0)
        {
            SwitchStatus(EnumEnmyStatus.Attack);
        }
        else
        {
            SwitchStatus(EnumEnmyStatus.Move);
        }
        return true;
    }

    public override void _Ready()
    {
        view = GetNode<AnimatedSprite2D>(NameConstants.View);
        viewChanged = GetNodeOrNull<AnimatedSprite2D>(NameConstants.ViewChanged);
        if (viewChanged == null)
        {
            viewChanged = GetNode<AnimatedSprite2D>(NameConstants.View);
        }
        viewExtra = GetNodeOrNull<AnimatedSprite2D>(NameConstants.ViewExtra);
        iceFx = GetNode<AnimatedSprite2D>(NameConstants.IceFreeze);
        fatherNode = GetParent<Node2D>();
        redEye = GetNode<Node2D>(NameConstants.ViewRed);
        EndRedMode();
        Init();
    }

    public float HasInitAction() => IntroAniTime;
    public bool DoInitAction()
    {
        // SwitchStatus(EnumEnmyStatus.Intro);
        // __introAniTime = 0.00001f;
        StartIntro();
        // StartIntro();
        return true;
    }
    public float HasDieAction() => OutroAniTime;
    public bool DoDieAction()
    {
        SwitchStatus(EnumEnmyStatus.Outro);
        return true;
    }
    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts)
    {
        throw new NotImplementedException();
    }
    public float GetAnimationSpeedScaleNoCold() => 1f;

    bool IsChanged = false;
    float __introAniTime = 0f;
    float __changeTime = 0f;
    public override void _Process(double delta)
    {
        // 执行初始化动作
        /*
        if (__introAniTime > 0f)
        {
            __introAniTime += (float)delta;
            DoingIntro((float)delta);
            if (__introAniTime >= IntroAniTime)
            {
                __introAniTime = 0f;
                EndIntro();
            }
        }
        // 执行改变动作
        if (__changeTime > 0f)
        {
            __changeTime += (float)delta;
            DoingChange((float)delta);
            if (__changeTime > HasChangeAction())
            {
                __changeTime = 0f;
                EndChangeAction();
                EndChange();
            }
        }
        */
    }

    // 看见植物
    public void SeeTarget(IObj obj) => RunningJudgeChanging(EnumWhenChangingType.SeePlansFirst);
    
    void RunningJudgeChanging(EnumWhenChangingType actionType)
    {
        if (actionType != WhenChangingType) return;
        if (IsChanged) return;
        StartChangeAction();
    }
    public int IsInAttack() => (fatherNode as IEnmy).IsInAttack();
    public void SetInAttack(bool isa) { (fatherNode as IEnmy).SetInAttack(isa); }
    /**
        特殊化处理
    */
    [Export]
    public float IntroAniTime = 0.001f;
    [Export]
    public float OutroAniTime = 0.66f;
    [Export]
    public EnumWhenChangingType WhenChangingType = EnumWhenChangingType.SeePlansFirst;
    // 进场
    async void StartIntro()
    {
        // 开始走路
        this.CallDeferred(nameof(EndIntro));
    }
    void DoingIntro(float delta)
    {
        //
    }
    void EndIntro()
    {
        IWorking iw = fatherNode as IWorking;
        // 开启工作
        if (iw != null) iw.SetWorkingMode(true);
        // 开始走路
        SwitchStatus(EnumEnmyStatus.Move);
        // 尝试咆哮
        EnmyCenter.Instance?.PlayAlonePaoxiao(ObjName, this.GlobalPosition);
    }
    // 改变
    void StartChange()
    {
    }
    void DoingChange(float delta)
    {
        //
    }
    void EndChange()
    {
        //  
        // GD.Print("ZombiSAndExtra.EndChange");
    }
}
