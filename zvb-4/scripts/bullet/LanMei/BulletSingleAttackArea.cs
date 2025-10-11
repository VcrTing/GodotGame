using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletSingleAttackArea : Area2D, IInit
{
    AnimatedSprite2D view;
    IBulletBase bulletBase;
    IObj myObj = null;
    IAttack myAttack = null;
    EnumObjType myType;
    EnumHurts enumHurts;
    public override void _Ready()
    {
        view = GetNode<AnimatedSprite2D>(NameConstants.View);
        view.Play(NameConstants.Default);
        view.Visible = true;
        AreaEntered += OnAreaEntered;
        //
        myObj = GetParent<IObj>();
        myAttack = myObj as IAttack;
        bulletBase = myObj as IBulletBase;
        ViewTool.RotationALittleByX(view, bulletBase.GetDirection());
        //
        myType = myObj.GetEnumObjType();
        enumHurts = myObj is IBulletBase bullet ? bullet.GetHurtType() : EnumHurts.Cold;
        //
        EnableCollision(false);
    }

    public bool Init(string objName = null)
    {
        EnableCollision(true);
        view.Visible = false;
        return true;
    }

    // 开关碰撞检测
    public void EnableCollision(bool enable)
    {
        Monitoring = enable;
        SetProcess(enable);
    }
    float collectionTime = 0.1f; // 收集时间
    public override void _Process(double delta)
    {
        // Rotation
        ViewTool.RotationALittleByX(view, bulletBase.GetDirection());
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

    
    // 群体伤害判定方法
    private void HandleGroupDamage(Area2D area)
    {
        if (area == null) return;
        DoTakeDamage(area, myAttack.GetDamage() / 3);
    }

    // 单独伤害方法
    private void HandleSingleDamage(Area2D area)
    {
        if (area == null) return;
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
