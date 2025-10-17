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
        enumHurts = myObj is IBulletBase bullet ? bullet.GetHurtType() : EnumHurts.Cold;
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
        int d = myAttack.GetDamage();
        isWorking = true;
        if (!firstHit)
        {
            firstHit = true;
        }
        else
        {
            d = d / 3;
        }
        ObjTool.TakeDamage(area, myType, d, enumHurts);
    }

    IObj myObj = null;
    IAttack myAttack = null;
    EnumHurts enumHurts = EnumHurts.Cold;
    EnumObjType myType = EnumObjType.Plans;
}
