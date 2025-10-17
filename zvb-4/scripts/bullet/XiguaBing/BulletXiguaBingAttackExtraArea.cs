using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletXiguaBingAttackExtraArea : Area2D
{
    private readonly System.Collections.Generic.HashSet<Area2D> enteredAreas = new();

    AnimatedSprite2D view;

    public override void _Ready()
    {
        view = GetNode<AnimatedSprite2D>(NameConstants.ViewExtra);
        view.Visible = false;
        AreaEntered += OnAreaEntered;
        EnableCollision(false);
        //
        myAttack = GetParent<IAttack>();
        myObj = GetParent<IObj>();
        enumHurts = myObj is IBulletBase bullet ? bullet.GetHurtType() : EnumHurts.Cold;
        myType = myObj.GetEnumObjType();
    }

    public void Init()
    {
        EnableCollision(true);
        view.Visible = true;
        view.Play(NameConstants.Default);
        // Rotation
        IBulletBase bulletBase = GetParent<IBulletBase>();
        ViewTool.RotationALittleByX(view, bulletBase.GetDirection());
    }
    public void EnableCollision(bool enable)
    {
        Monitoring = enable;
        SetProcess(enable);
    }

    private void OnAreaEntered(Area2D area)
    {
        if (!enteredAreas.Contains(area))
        {
            isWorking = true;
            enteredAreas.Add(area);
            int d = myAttack.GetDamageExtra();
            ObjTool.TakeDamage(area, myType, d, enumHurts);
        }
    }
    bool isWorking = false;
    float collectionTime = 0.08f; // 收集时间
    public override void _Process(double delta)
    {
        if (isWorking)
        {
            timerElapsed += (float)delta;
            if (timerElapsed >= collectionTime)
            {
                Die();
            }
        }
    }
    private float timerElapsed = 0f;
    private bool hasAttacked = false;
    
    IObj myObj = null;
    IAttack myAttack = null;
    EnumHurts enumHurts = EnumHurts.Cold;
    EnumObjType myType = EnumObjType.Plans;

    bool isDie = false;
    async void Die()
    {
        if (isDie) return;
        isDie = true;
        await ToSignal(GetTree().CreateTimer(0.32f), "timeout");
        QueueFree();
    }
    
}
