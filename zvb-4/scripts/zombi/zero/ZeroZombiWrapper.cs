using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;


public partial class ZeroZombiWrapper : Node2D, IObj, IMove, IWorking, IStatus, IBeHurt, IAttack, IInit, IEnmy
{
    IActionExtra actionExtra = null;
    Node2D bodyNode = null;
    float introAniTime = 0f;
    float outroAniTimer = 0f;

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
        InitViewScale = viewscale + Scale.X;
        InitAttackSpeedScale = attackspeedscale;

        Scale = new Vector2(InitViewScale, InitViewScale);
    }

    public override void _Ready()
    {
        scaleMax = Scale.X;
        scaleMin = ViewTool.GetYouMinScale(scaleMax);
        //
        isMoving = false;
        //
        Init(objName);
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
    float scaleMin = GameContants.MinScale;
    float scaleMax = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, scaleMin, scaleMax); return true;
    }
    public async void DoIntro()
    {
        introAniTime = actionExtra.HasInitAction();
        if (introAniTime > 0f)
        {
            actionExtra.DoInitAction();
            await ToSignal(GetTree().CreateTimer(introAniTime), "timeout");
        }
        SetWorkingMode(true);
        IEnmy e = bodyNode as IEnmy;
        if (e != null)
        {
            e.SwitchStatus(EnumEnmyStatus.Move);
        }
    }
    public float DoOutro()
    {
        PauseMove();
        SetWorkingMode(false);
        float outroAniTime = actionExtra.HasDieAction();
        if (outroAniTime > 0f)
        {
            actionExtra.DoDieAction();
        }
        return outroAniTime;
    }
    public bool Die()
    {
        // Die();
        return true;
    }
    [Export]
    public EnumObjType enumObjType = EnumObjType.Zombie;
    public EnumObjType GetEnumObjType() => enumObjType;
    [Export]
    public string objName = EnmyTypeConstans.ZombiMuTong;
    public string GetObjName() => objName;
    public void SetObjName(string name) => objName = name;
    public bool Init(string name = null)
    {
        //
        Scale = new Vector2(InitViewScale, InitViewScale);
        // 加载纹理
        bool isOk = EnmyTypeConstans.GenerateZombiTexture(this, objName);
        if (!isOk) QueueFree();
        //
        actionExtra = GetNodeOrNull<IActionExtra>(NameConstants.Body);
        if (actionExtra != null)
        {
            DoIntro();
        }
        bodyNode = actionExtra as Node2D; // GetNodeOrNull<IStatus>(NameConstants.Body);
        IInit ci = bodyNode as IInit;
        if (ci != null)
        {
            ci.Init(objName);
        }
        return true;
    }
    bool isMoving = false;
    public void PauseMove() { isMoving = false;  }
    public bool SetMyPosition(Vector2 pos)
    {
        if (isMoving == false) return false;
        Position = pos;
        return true;
    }
    public void StartMove() { isMoving = true;  }
    public Vector2 GetMyPosition() => Position;
    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        if (isWorking)
        {
            SwitchStatus(EnumEnmyStatus.Move);
        }
    }
    // 冰冻
    public bool DoFreeze(float time)
    {
        iceFreezeTime += time;
        __freezeTime += 0.01f;
        __coldTime = 0f;
        // GD.Print("1. 冰冻 =" + iceFreezeTime);
        //
        moveSpeedScale = 0f;
        attackSpeedScale = 0f;
        animationSpeedScale = 0f;
        (bodyNode as IStatus).DoFreeze(iceFreezeTime);
        return true;
    }
    public bool ReleaseFreeze()
    {
        // GD.Print("2. 解开冰冻");
        iceFreezeTime = 0f;
        iceColdTime = 0f;
        (bodyNode as IStatus).ReleaseFreeze();
        DoCold(iceColdTimeInit);
        return true;
    }
    // 减速
    public bool DoCold(float time)
    {
        iceColdTime += time;
        if (iceFreezeTime <= 0f)
        {
            iceColdTime = time;
        }
        __coldTime += 0.01f;
        // GD.Print("3. 减速 =" + iceColdTime);
        //
        moveSpeedScale = GameContants.ColdScale;
        attackSpeedScale = GameContants.ColdScale;
        animationSpeedScale = GameContants.ColdScale;
        (bodyNode as IStatus).DoCold(time);
        return true;
    }
    // 重置
    public bool ReleaseCold()
    {
        __coldTime = 0f;
        __freezeTime = 0f;
        iceColdTime = 0f;
        iceFreezeTime = 0f;
        // GD.Print("4. 重置减速");
        //
        moveSpeedScale = 1f;
        attackSpeedScale = 1f;
        animationSpeedScale = 1f;
        (bodyNode as IStatus).ReleaseCold();
        return true;
    }
    /**

    */
    float moveSpeedScale = 1f;
    public float GetMoveSpeedScale()
    {
        return moveSpeedScale * InitMoveSpeedScale;
    }
    float attackSpeedScale = 1f;
    public float GetAttackSpeedScale()
    {
        return attackSpeedScale * InitAttackSpeedScale;
    }
    float animationSpeedScale = 1f;
    public float GetAnimationSpeedScale()
    {
        return animationSpeedScale * InitMoveSpeedScale;
    }
    //
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!isWorking) return false;
        // 伤害缩放
        damage = (int)(damage * InitBeHurtScale);
        // 处理冰冻
        if (enumHurts == EnumHurts.IceFreeze)
        {
            DoFreeze(iceFreezeTimeInit);
        }
        // 其他攻击
        else
        {
            // 处理伤害
            bool isAlive = true;
            IBeHurt beHurt = bodyNode as IBeHurt;
            if (beHurt != null)
            {
                isAlive = beHurt.BeHurt(objType, damage, enumHurts);
            }
            if (isAlive == false)
            {
                Die(objType, damage, enumHurts);
            }
            
            // 处理 可减速攻击
            if (enumHurts == EnumHurts.Cold)
            {
                DoCold(iceColdTimeInit);
            }
        }
        return true;
    }
    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        //
        GameStatistic.Instance?.AddZombieDead();
        //
        IEnmy e = bodyNode as IEnmy;
        if (e != null)
        {
            e.SwitchStatus(EnumEnmyStatus.Die);
        }
        // 关闭碰撞
            Node2D beHurtArea = GetNodeOrNull<Node2D>(NameConstants.BeHurtArea);
        if (beHurtArea != null)
        {
            beHurtArea.QueueFree();
        }
        // 删掉移动
        Node2D m = GetNodeOrNull<Node2D>(NameConstants.Move);
        if (m != null)
        {
            m.QueueFree();
        }
        // 停止移动。播放死亡动画。
        float dieTime = DoOutro();
        // 掉落奖励
        IWhenDie wdr = GetNodeOrNull<IWhenDie>(NameConstants.WorkingDumpReword);
        if (wdr != null)
        {
            wdr.WorkingWhenDie(objName);
        }
        await ToSignal(GetTree().CreateTimer(dieTime), "timeout");
        QueueFree();
        return true;
    }
    
    public int GetDamage()
    {
        int v = (int)(EnmyTypeConstans.GetZombieDamage(objName));
        return v;
    }

    public int GetDamageExtra()
    {
        return (int)(EnmyTypeConstans.GetZombieDamage(objName));
    }

    //

    public EnumEnmyStatus GetStatus()
    {
        return ((bodyNode as IEnmy) != null) ? (bodyNode as IEnmy).GetStatus() : EnumEnmyStatus.None;
    }

    public void SwitchStatus(EnumEnmyStatus status)
    {
        if ((bodyNode as IEnmy) != null)
        {
            if (status == EnumEnmyStatus.Move)
            {
                StartMove();
            }
            else
            {
                PauseMove();
            }
            (bodyNode as IEnmy).SwitchStatus(status);
        }
    }

    public EnumMoveType GetEnumMoveType()
    {
        return ((bodyNode as IEnmy) != null) ? (bodyNode as IEnmy).GetEnumMoveType() : EnumMoveType.LineWalk;
    }

    public bool IsWorking() => isWorking;

    public bool CanAttack()
    {
        return isWorking && __freezeTime <= 0f;
    }
}
