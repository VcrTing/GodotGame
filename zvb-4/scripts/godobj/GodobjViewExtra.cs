using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class GodobjViewExtra : Node2D, IHealth
{
    private AnimatedSprite2D _aliveSprite;
    [Export]
    public EnumWhatYouObj WhatYouObj { get; set; } = EnumWhatYouObj.None;

    
    int HealthInit = (int)EnumHealth.Two;
    int Health = (int)EnumHealth.Two;
    public override void _Ready()
    {
        // 切换默认动画
        GodotTool.SwitchAnimatedSprite(this, NameConstants.Alive);

        // 获取父对象的objType
        var parent = GetParent();
        if (parent is GodobjWrapper wrapper)
        {
            HealthInit = ObjTool.GetYouExtraHealth(wrapper.objType, wrapper.objName);
            Health = HealthInit;
            DoingDie();
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
    bool DoingDie()
    {
        if (Health <= 0)
        {
            // Health = 0;
            isDie = true;
            DieFaker();
            return true; // 表示已经死亡
        }
        return false;
    }
    public int CostHealth(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        Health -= damage;
        PlayHurtEffect(objType, damage, enumHurts);
        SwitchViewIfHealthHalf();
        DoingDie(); // 仍然存活
        int yichu = 0;
        if (Health < 0) yichu = -Health;
        return yichu;
    }

    async void PlayHurtEffect(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        // 根据攻击类型，播放不同的受伤音效
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, WhatYouObj, this.GlobalPosition);
        HurtFx();
    }

    async void HurtFx()
    {
        //
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
