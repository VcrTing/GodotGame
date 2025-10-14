using Godot;
using System;

public partial class BackToMainButton : TextureButton
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
