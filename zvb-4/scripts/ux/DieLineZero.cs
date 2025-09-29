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
        string name = area.Name;
        if (name == NameConstants.AttackArea)
        {
            IObj obj = area.GetParent<IObj>();
            if (obj != null && obj.GetEnumObjType() == EnumObjType.Zombie)
            {
                GD.Print($"DieLineZero: 游戏结束 被僵尸 {obj.GetObjName()} 进入");
                Over();
            }
        }
        // throw new NotImplementedException();
    }

    async void Over()
    {
        SoundFxController.Instance?.PlayFx("Ux/over", "game_over_ready", 4);
        BgmController.Instance?.PauseBgm();
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        SoundFxController.Instance?.PlayFx("Ux/over", "game_over", 3);
        await ToSignal(GetTree().CreateTimer(5f), "timeout");
        // 场景重置
        GetTree().ReloadCurrentScene();
    }

    private void OnBodyEntered(Node body)
    {
        if (body != null && body.Name == NameConstants.AttackBody)
        {
            GD.Print("游戏结束");
            Over();
        }
    }
}
