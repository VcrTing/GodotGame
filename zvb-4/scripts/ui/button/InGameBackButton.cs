using Godot;
using System;
using ZVB4.Conf;

public partial class InGameBackButton : TextureButton
{
	public override void _Ready()
	{
        ProcessMode = ProcessModeEnum.Always;
		this.Pressed += OnBackButtonPressed;
	}

    private void OnBackButtonPressed()
    {
        UiTool.BackToMainScene(this);
	}
}
