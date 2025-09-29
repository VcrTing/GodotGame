using Godot;
using System;
using ZVB4.Conf;

public partial class InGameBackButton : TextureButton
{
	public override void _Ready()
	{
		this.Pressed += OnBackButtonPressed;
	}

    private async void OnBackButtonPressed()
    {
        GD.Print("InGameBackButton Pressed!");
        UiTool.BackToMainScene(this);
	}
}
