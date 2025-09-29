using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class GodAttackArea : Area2D
{
    IObj myObj => GetParent<IObj>();
    IMove myMove => GetParent<IMove>();
    IAttack myAttack => GetParent<IAttack>();

	private Timer _hurtTimer;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    private void OnAreaExited(Area2D area)
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
            attackWho = null;
            myMove?.StartMove();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        DoAttack(area);
	}

    Area2D attackWho = null;
    void DoAttack(Area2D area)
    {
        if (area == null) return;
        IObj p = area.GetParent<IObj>();
        if (p != null)
        {
            attackSpeed = EnmyTypeConstans.GetBaitSpeed(myObj.GetObjName());
            // 
            EnumObjType tp = p.GetEnumObjType();
            if (tp == myObj.GetEnumObjType())
            {
                return;
            }

            attackWho = area;
        }
    }

    void EndAttack(Area2D area)
    {
        IObj p = area.GetParent<IObj>();
        if (p == null) return;
        GD.Print($"GodAttackArea EndHurtPlans {p.GetObjName()}");
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
            myMove?.PauseMove();
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

    void TryAttack(IObj p, double delta)
    {
        EnumObjType t = p.GetEnumObjType();
        if (t == EnumObjType.Plans)
        {
            // 伤害植物
            AttackPlans(p);
        }
    }
    void AttackPlans(IObj plans)
    {
        IBeHurt beHurt = plans as IBeHurt;
        if (beHurt != null)
        {
            // 造成伤害
            _ = beHurt.BeHurt(myObj.GetEnumObjType(), myAttack.GetDamage(), EnumHurts.Bait);
            // GD.Print($"GodAttackArea HurtPlans {plans.GetObjName()}");
        }
    }
}