using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansJianGuoView : AnimatedSprite2D, IHealth
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
        AnimationTool.DoAniExtraLiveHp(this, Health, HealthInit);
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
            // 0.5秒后调用DieFaker
            await ToSignal(GetTree().CreateTimer(AnimationConstants.GetViewDieAniTime(myObj)), "timeout");
            DieFaker();
        }
    }

    EnumHurts lastHurt = EnumHurts.Pea;
    //
    public int CostHealth(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (objType == myObj.GetEnumObjType()) return Health;
        //
        lastHurt = enumHurts;
        Health -= damage;
        PlayHurtEffect(objType, damage, enumHurts);
        SwitchViewByHealth();
        int yichu = 0;
        if (Health < 0) yichu = -Health;
        DoingDie(); // 仍然存活
        return yichu;
    }
    //
    void PlayHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, WhatYouObj, this.GlobalPosition);
        HurtFx();
    }

    bool isInHurting = false;
    async void HurtFx()
    {
        if (isInHurting) return;
        isInHurting = true;
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.7f);
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
        await ToSignal(GetTree().CreateTimer(0.24f), "timeout");
        isInHurting = false;
    }
    void DieFaker()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
    }

}
