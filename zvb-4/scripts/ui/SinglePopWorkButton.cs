using Godot;
using System;

public partial class SinglePopWorkButton : TextureButton
{
    // 维护一个字符串
	// game_stop
	// game_rule_detail
    [Export]
    public string Key = "game_stop";

    [Export]
    public bool IsOpen = true;

	public override void _Ready()
	{
        ProcessMode = ProcessModeEnum.Always;
		Pressed += OnButtonPressed;
        this.MouseEntered += () => SoundUiController.Instance.Hover();
		Visible = true;
	}

	private void OnButtonPressed()
	{
		HandleByKey(Key);
	}

	private void HandleByKey(string key)
    {
		if (IsOpen)
		{
			SoundUiController.Instance.Sure();
		}
		else
		{
			SoundUiController.Instance.Back();
		}
		switch (key)
		{
			case "game_rule_detail":
				GameRuleDetailPopup.Instance?.Change(IsOpen);
				break;
			case "game_stop":
				GameStopPopup.Instance?.Change(IsOpen);
				break;
			default:
				GD.Print($"SinglePopOpenButton: 未知key: {key}");
				break;
		}
	}
}
