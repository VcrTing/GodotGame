using Godot;
using System;

public partial class BackToMainButton : TextureButton
{
	public override void _Ready()
	{
		this.Pressed += OnBackButtonPressed;
	}

    private async void OnBackButtonPressed()
    {
        GD.Print("BackToMainButton Pressed!");
        UiTool.BackToMainScene(this);
	}
}
