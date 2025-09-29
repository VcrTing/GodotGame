using Godot;
using System;
using ZVB4.Conf;

using ZVB4.Interface;
using ZVB4.Tool;

public partial class GodobjView : Node2D, IHealth
{

    [Export]
    public EnumWhatYouObj WhatYouObj { get; set; } = EnumWhatYouObj.None;

    int HealthInit = (int)EnumHealth.Two;
    int Health = (int)EnumHealth.Two;

    IObj myObj => GetParent<IObj>();
    public override void _Ready()
    {
        GodotTool.SwitchAnimatedSprite(this, NameConstants.Alive);
        HealthInit = ObjTool.GetYouHealth(myObj.GetEnumObjType(), myObj.GetObjName());
        Health = HealthInit;
        DoingDie();
        SwitchViewByHealth();
    }

    void SwitchViewByHealth()
    {
        if (Health <= HealthInit / 2)
        {
            GodotTool.SwitchAnimatedSprite(this, NameConstants.AliveHalf);
        }
        else
        {
            GodotTool.SwitchAnimatedSprite(this, NameConstants.Alive);
        }
    }
    void SwitchViewIfHealthHalf()
    {
        if (Health <= HealthInit / 2)
        {
            GodotTool.SwitchAnimatedSprite(this, NameConstants.AliveHalf);
        }
    }
    bool isDie = false;

    public bool IsDie()
    {
        return isDie;
    }

    public bool UpHealth(int heal)
    {
        Health += heal;
        if (Health > HealthInit) Health = HealthInit;
        return true;
    }

    async void DoingDie()
    {
        if (Health <= 0)
        {
            isDie = true;
            // 隐藏所有子节点
            GodotTool.SwitchAnimatedSprite(this, NameConstants.Die);
            // GD.Print($"{Name} 死亡. ");
            // 0.5秒后调用DieFaker
            await ToSignal(GetTree().CreateTimer(AnimationConstants.GetViewDieAniTime(myObj)), "timeout");
            DieFaker();
        }
    }
    public int CostHealth(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        Health -= damage;
        PlayHurtEffect(objType, damage, enumHurts);
        SwitchViewIfHealthHalf();
        int yichu = 0;
        if (Health < 0) yichu = -Health;
        DoingDie(); // 仍然存活
        return yichu;
    }

    void PlayHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, WhatYouObj, this.GlobalPosition);
        HurtFx();
    }


    async void HurtFx()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.7f);
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        if (isDie) return;
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
    }

    void DieFaker()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
    }
}
