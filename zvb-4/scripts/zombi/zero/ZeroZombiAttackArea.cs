using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ZeroZombiAttackArea : Area2D
{
    IObj myObj => GetParent<IObj>();
    IAttack myAttack => GetParent<IAttack>();

    private Timer _hurtTimer;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    private void OnAreaExited(Area2D area)
    {
        if (area == null) return;
        try
        {
            IObj p = area.GetParent<IObj>();
            if (p != null)
            {
                EnumObjType tp = p.GetEnumObjType();
                if (tp == myObj.GetEnumObjType())
                {
                    return;
                }
                EndAttack(area);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"ZeroZombiAttackArea OnAreaExited error: {ex.Message}");
            return;
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area == null) return;
        try
        {
            StartAttack(area);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"ZeroZombiAttackArea OnAreaEntered error: {ex.Message}");
            return;
        }
    }

    Area2D attackWho = null;
    void StartAttack(Area2D area)
    {
        IObj p = area.GetParent<IObj>();
        if (p != null)
        {
            EnumObjType tp = p.GetEnumObjType();
            if (tp == myObj.GetEnumObjType())
            {
                return;
            }
            attackSpeed = EnmyTypeConstans.GetBaitSpeed(myObj.GetObjName());
            attackWho = area;
        }
    }

    async void EndAttack(Area2D area)
    {
        attackWho = null;
        // 恢复行走动画
        SwitchMoveStatus();
        // 重启一下碰撞检测
        EnableCollision(false);
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        EnableCollision(true);
    }

    public void EnableCollision(bool enable)
    {
        try
        {
            Monitoring = enable;
            SetDeferred("monitoring", enable);
        }
        catch (Exception ex)
        {
            // 使用SetDeferred避免信号回调期间的状态变更错误
            SetDeferred("monitoring", enable);
            GD.PrintErr($"ZeroZombiAttackArea EnableCollision error: {ex.Message}");
        }
        SetProcess(enable);
    }

    //
    double attackElapsed = 0.0;
    double attackIntervalElapsed = 0.0;
    float attackWhenStart = EnmyTypeConstans.BaseBaitLazyStart;
    float attackSpeed = EnmyTypeConstans.BaseBaitSpeed;
    public override void _Process(double delta)
    {
        if (attackWho != null)
        {
            IObj p = attackWho.GetParent<IObj>();
            if (p != null)
            {
                // 持续伤害计时
                attackElapsed += delta;
                if (attackElapsed >= attackWhenStart)
                {
                    attackIntervalElapsed += delta;
                    if (attackIntervalElapsed >= attackSpeed)
                    {
                        TryAttack(p, delta);
                        attackIntervalElapsed = 0.0;
                    }
                }
            }
        }
        else
        {
            // 停止攻击，重置计时
            attackElapsed = 0.0;
            attackIntervalElapsed = 0.0;
        }
    }

    string lastAnimationName = AnimationConstants.AniWalk;
    void TryAttack(IObj p, double delta)
    {
        EnumObjType t = p.GetEnumObjType();
        if (t == EnumObjType.Plans)
        {
            // 伤害植物
            AttackPlans(p);
            SwitchAttackStatus();
        }
    }
    void AttackPlans(IObj plans)
    {
        IBeHurt beHurt = plans as IBeHurt;
        if (beHurt != null)
        {
            // 造成伤害
            _ = beHurt.BeHurt(myObj.GetEnumObjType(), myAttack.GetDamage(), EnumHurts.Bait);
        }
    }

    void SwitchAttackStatus()
    {
        // 尝试播放吃动画
        if (lastAnimationName != AnimationConstants.AniAttack)
        {
            IEnmy me = myObj as IEnmy;
            if (me != null)
            {
                me.SwitchStatus(EnumEnmyStatus.Attack);
                lastAnimationName = AnimationConstants.AniAttack;
            }
        }
    }
    void SwitchMoveStatus()
    {
        // 尝试播放走路动画
        if (lastAnimationName != AnimationConstants.AniWalk)
        {
            IEnmy me = myObj as IEnmy;
            if (me != null)
            {
                me.SwitchStatus(EnumEnmyStatus.Move);
                lastAnimationName = AnimationConstants.AniWalk;
            }
        }
    }
}
