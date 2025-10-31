using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZombiCWrapper : Node2D, IInit, IObj, IWorking, IMove, IBeHurt, IStatus, IEnmy, IAttack
{
    bool _isPlayColdSound = false;
    public void PlayColdingSound()
    {
        if (_isPlayColdSound) return;
        _isPlayColdSound = true;
        SoundOneshotController.Instance?.PlayFx("Fx/coldstart", "BeCold", 4, 1f, GlobalPosition, 5f);
    }
    //
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!isWorking) return false;
        // 处理冰冻
        if (enumHurts == EnumHurts.IceFreeze)
        {
            DoFreeze(iceFreezeTimeInit);
        }
        else
        {
            // 伤害缩放
            damage = (int)(damage * GetBeHurtScale());
            // 处理伤害
            IBeHurt beHurt = bodyNode as IBeHurt;
            if (beHurt != null)
            {
                bool isAlive = beHurt.BeHurt(objType, damage, enumHurts);
                if (!isAlive) { Die(objType, damage, enumHurts); }
            }
            // 处理减速
            if (ZombiTool.CanBeCold(GetWhatYouObj(), enumHurts)) 
            {
                DoCold(iceColdTimeInit);
                PlayColdingSound(); 
            }
        }
        return true;
    }
    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        ZombiTool.RunningWhenDie(this, bodyNode);
        // 播放死亡效果
        float dieTime = 0.4f;
        if (actionExtra != null)
        {
            if (enumHurts != EnumHurts.Boom) // 炸弹不播放死亡动画
            {
                dieTime = actionExtra.HasDieAction();
                if (dieTime > 0f)
                {
                    actionExtra.DoDieAction();
                    await ToSignal(GetTree().CreateTimer(dieTime), "timeout");
                }
            }
            else
            {
                await ToSignal(GetTree().CreateTimer(dieTime), "timeout");
            }
        }
        QueueFree();
        return true;
    }
    public bool Die() => true;
    [Export]
    public EnumObjType enumObjType = EnumObjType.Zombie;
    public EnumObjType GetEnumObjType() => enumObjType;
    [Export]
    public string objName = EnmyTypeConstans.ZombiS;
    public string GetObjName() => objName;
    public void SetObjName(string name) => objName = name;
    //
    Node2D bodyNode = null;
    ICcActionExtra actionExtra = null;
    public bool Init(string name = null)
    {
        Scale = new Vector2(InitViewScale, InitViewScale);
        // 加载纹理
        bool isOk = EnmyTypeConstans.GenerateZombiTexture(this, objName);
        if (!isOk) QueueFree();
        //
        bodyNode = GetNodeOrNull<Node2D>(NameConstants.Body);
        actionExtra = bodyNode as ICcActionExtra;
        if (actionExtra != null)
        {
            // 有 intro 动画
            float introAniTime = actionExtra.HasInitAction();
            if (introAniTime > 0f)
            {
                actionExtra.DoInitAction();
            }
        }
        return bodyNode != null;
    }
    float iceFreezeTime = 0f;
    float iceFreezeTimeInit = GameContants.IceTime;
    float __freezeTime = 0f;
    float iceColdTime = 0f;
    float iceColdTimeInit = GameContants.ColdTime;
    float __coldTime = 0f;
    public override void _Process(double delta)
    {
        // 冰冻
        if (__freezeTime > 0f)
        {
            __freezeTime += (float)delta;
            if (__freezeTime > iceFreezeTime)
            {
                __freezeTime = 0f;
                ReleaseFreeze();
            }
        }
        // 减速
        if (__coldTime > 0f)
        {
            __coldTime += (float)delta;
            if (__coldTime > iceColdTime)
            {
                __coldTime = 0f;
                ReleaseCold();
            }
        }
        //
        AdjustView();
    }
    public override void _Ready()
    {
        isMoving = false;
        SetWorkingMode(false);
    }
    bool isMoving = false;
    public void PauseMove() { isMoving = false; }
    public bool SetMyPosition(Vector2 pos)
    {
        if (isMoving == false) return false;
        Position = pos;
        return true;
    }
    public void StartMove() { isMoving = true; }
    public Vector2 GetMyPosition() => Position;
    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        Area2D attack = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
        Area2D hurt = GetNodeOrNull<Area2D>(NameConstants.BeHurtArea);
        Area2D eye = GetNodeOrNull<Area2D>(NameConstants.EyeArea);
        if (attack != null) attack.Monitoring = working;
        if (hurt != null) hurt.Monitoring = working;
        if (eye != null) eye.Monitoring = working;
    }
    public bool IsWorking() => isWorking;
    public EnumMoveType GetEnumMoveType()
    {
        return ((bodyNode as IEnmy) != null) ? (bodyNode as IEnmy).GetEnumMoveType() : EnumMoveType.LineWalk;
    }
    public bool DoFreeze(float time)
    {
        iceFreezeTime += time;
        __freezeTime += 0.01f;
        __coldTime = 0f;
        //
        moveSpeedScale = 0f;
        attackSpeedScale = 0f;
        animationSpeedScale = 0f;
        (bodyNode as IStatus).DoFreeze(iceFreezeTime);
        return true;
    }
    public bool DoCold(float time)
    {
        iceColdTime += time;
        if (iceFreezeTime <= 0f)
        {
            iceColdTime = time;
        }
        __coldTime += 0.01f;
        //
        moveSpeedScale = GameContants.ColdScale;
        attackSpeedScale = GameContants.ColdScale;
        animationSpeedScale = GameContants.ColdScale;
        (bodyNode as IStatus).DoCold(time);
        return true;
    }
    public bool ReleaseFreeze()
    {
        iceFreezeTime = 0f;
        iceColdTime = 0f;
        (bodyNode as IStatus).ReleaseFreeze();
        DoCold(iceColdTimeInit);
        return true;
    }
    public bool ReleaseCold()
    {
        __coldTime = 0f;
        __freezeTime = 0f;
        iceColdTime = 0f;
        iceFreezeTime = 0f;
        //
        moveSpeedScale = 1f;
        attackSpeedScale = 1f;
        animationSpeedScale = 1f;
        (bodyNode as IStatus).ReleaseCold();
        return true;
    }

    [Export]
    public float InitMoveSpeedScale = 1f;
    [Export]
    public float InitBeHurtScale = 1f;
    [Export]
    public float InitViewScale = 1f;
    [Export]
    public float InitAttackSpeedScale = 1f;
    public void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale)
    {
        InitMoveSpeedScale = movespeedscale;
        InitBeHurtScale = behurtscale;
        InitViewScale = viewscale;
        InitAttackSpeedScale = attackspeedscale;

        float sx = Scale.X * InitViewScale;
        Scale = new Vector2(sx, sx);

        scaleMax = Scale.X;
        scaleMin = ViewTool.GetYouMinScale(scaleMax, InitViewScale);
    }
    float moveSpeedScale = 1f;
    public float GetMoveSpeedScale()
    {
        return moveSpeedScale * InitMoveSpeedScale * RedEyeScale;
    }
    float attackSpeedScale = 1f;
    public float GetAttackSpeedScale()
    {
        return attackSpeedScale * InitAttackSpeedScale * RedEyeScale;
    }
    float animationSpeedScale = 1f;
    public float GetAnimationSpeedScale()
    {
        return animationSpeedScale * InitMoveSpeedScale * RedEyeScale;
    }
    public float GetAnimationSpeedScaleNoCold()
    {
        return 1f * InitMoveSpeedScale * RedEyeScale;
    }
    public float GetBeHurtScale()
    {
        return 1f * InitBeHurtScale;
    }
    float scaleMin = GameContants.MinScale;
    float scaleMax = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, scaleMin, scaleMax); return true;
    }

    public EnumEnmyStatus GetStatus() => (bodyNode as IEnmy != null) ? (bodyNode as IEnmy).GetStatus() : EnumEnmyStatus.None;
    public void SwitchStatus(EnumEnmyStatus status) => (bodyNode as IEnmy)?.SwitchStatus(status);
    public void SeeTarget(IObj obj) => (bodyNode as IEnmy)?.SeeTarget(obj);
    public bool CanAttack() => (isWorking && __freezeTime <= 0f);
    public int GetDamage() => (int)(EnmyTypeConstans.GetZombieDamage(objName));
    public int GetDamageExtra()
    {
        throw new NotImplementedException();
    }

    float RedEyeScale = 1f;
    public void JudgeOpenRedEyeMode(float redeyeratio)
    {
        if (redeyeratio <= 0f) return;
        int i = GD.RandRange(0, 100);
        if (i <= (100 * redeyeratio)) StartRedMode();
    }
    public void StartRedMode()
    {
        RedEyeScale = EnmyTypeConstans.RedEyeScale;
        (bodyNode as IEnmy)?.StartRedMode();
    }
    public void EndRedMode()
    {
        RedEyeScale = 1f;
        (bodyNode as IEnmy)?.EndRedMode();
    }
    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts)
    {
        throw new NotImplementedException();
    }
    public EnumWhatYouObj GetWhatYouObj() => (EnumWhatYouObj)((bodyNode as IEnmy)?.GetWhatYouObj());

}
