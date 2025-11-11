using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class DieLineZero : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        try
        {
            if (area == null) return;
            string name = area.Name;
            if (name == NameConstants.AttackArea)
            {
                IObj obj = area.GetParent<IObj>();
                if (obj == null) return; 
                if (obj != null && obj.GetEnumObjType() == EnumObjType.Zombie)
                {
                    GD.Print($"DieLineZero: 游戏结束 被僵尸 {obj.GetObjName()} 进入");
                    Over();
                }
            }
        }
        catch (Exception e)
        {
            
        }
        // throw new NotImplementedException();
    }

    async void Over()
    {
        BgmController.Instance?.PauseBgm();
        SoundFxController.Instance?.PlayFx("Ux/die", "die_aoxiao", 4);
        await ToSignal(GetTree().CreateTimer(2f), "timeout");
        CapterDiePopup.Instance?.ShowPopup();
        // 暂停场景
        // GetTree().Paused = true;
    }

    private void OnBodyEntered(Node body)
    {
        if (body != null && body.Name == NameConstants.AttackBody)
        {
            Over();
        }
    }
}
