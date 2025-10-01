using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletXiguaBingAttackArea : Area2D
{
    AnimatedSprite2D view;
    IBulletBase bulletBase;

    public override void _Ready()
    {
        view = GetNode<AnimatedSprite2D>(NameConstants.View);
        view.Play(NameConstants.Default);
        view.Visible = true;
        AreaEntered += OnAreaEntered;
        bulletBase = GetParent<IBulletBase>();
        ViewTool.RotationALittleByX(view, bulletBase.GetDirection());
        //
        myAttack = GetParent<IAttack>();
        myObj = GetParent<IObj>();
        enumHurts = myObj is IBulletBase bullet ? bullet.GetHurtType() : EnumHurts.XiguaBing;
        myType = myObj.GetEnumObjType();
        //
        EnableCollision(false);
    }

    public void Init()
    {
        EnableCollision(true);
        view.Visible = false;
    }
    // 开关碰撞检测
    public void EnableCollision(bool enable)
    {
        Monitoring = enable;
        SetProcess(enable);
    }
    float collectionTime = 0.3f; // 收集时间
    public override void _Process(double delta)
    {
        // Rotation
        ViewTool.RotationALittleByX(view, bulletBase.GetDirection());
        // MMM
        if (isWorking)
        {
            timerElapsed += (float)delta;
            if (timerElapsed >= collectionTime)
            {
                QueueFree();
            }
        }
    }
    // 计时相关变量
    private float timerElapsed = 0f;

    bool isWorking = false;
    bool firstHit = false;
    private void OnAreaEntered(Area2D area)
    {
        isWorking = true;
        if (!firstHit)
        {
            HandleSingleDamage(area);
            firstHit = true;
        }
        else
        {
            HandleGroupDamage(area);
        }
    }

    IObj myObj = null;
    IAttack myAttack = null;
    EnumHurts enumHurts = EnumHurts.XiguaBing;
    EnumObjType myType = EnumObjType.Plans;
    
    // 群体伤害判定方法
    private void HandleGroupDamage(Area2D area)
    {
        if (area == null) return;
        // GD.Print($"组团伤害 HandleGroupDamage: {area.Name}, Damage: {myAttack.GetDamage() / 3}");
        DoTakeDamage(area, myAttack.GetDamage() / 3);
    }

    // 单独伤害方法
    private void HandleSingleDamage(Area2D area)
    {
        if (area == null) return;
        // GD.Print($"单体伤害 HandleSingleDamage: {area.Name}, Damage: {myAttack.GetDamage()}");
        DoTakeDamage(area, myAttack.GetDamage());
    }
    
    // 伤害处理方法
    void DoTakeDamage(Area2D area, int damage)
    {
        if (area is IHurtBase hurt)
        {
            hurt.TakeDamage(myType, damage, enumHurts);
        }
        else
        {
            var parent = area.GetParent();
            if (parent is IHurtBase hurtParent)
            {
                hurtParent.TakeDamage(myType, damage, enumHurts);
            }
        }
    }
}
