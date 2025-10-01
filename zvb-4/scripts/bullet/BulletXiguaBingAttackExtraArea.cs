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
        view = GetNode<AnimatedSprite2D>(NameConstants.View);
        view.Visible = false;
        AreaEntered += OnAreaEntered;
        EnableCollision(false);
        //
        myAttack = GetParent<IAttack>();
        myObj = GetParent<IObj>();
        enumHurts = myObj is IBulletBase bullet ? bullet.GetHurtType() : EnumHurts.XiguaBing;
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
        }
    }
    bool isWorking = false;
    float collectionTime = 0.08f; // 收集时间
    public override void _Process(double delta)
    {
        // MMM
        if (isWorking && enteredAreas.Count > 0)
        {
            timerElapsed += (float)delta;
            if (!hasAttacked && timerElapsed >= collectionTime)
            {
                hasAttacked = true;
                int d = myAttack.GetDamageExtra();
                foreach (var area in enteredAreas)
                {
                    DoTakeDamage(area, d);
                }
                QueueFree();
            }
        }
    }
    private float timerElapsed = 0f;
    private bool hasAttacked = false;
    
    IObj myObj = null;
    IAttack myAttack = null;
    EnumHurts enumHurts = EnumHurts.XiguaBing;
    EnumObjType myType = EnumObjType.Plans;
    
    // 伤害处理方法
    void DoTakeDamage(Area2D area, int damage)
    {
        try
        {
            // GD.Print($"溅射伤害 DoTakeDamage: {area.Name}, Damage: {damage}");
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
        catch (Exception ex)
        {
            GD.PrintErr($"溅射伤害 DoTakeDamage 异常: {ex.Message}");
        }
    }
}
