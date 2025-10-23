using Godot;
using System;

public partial class SingleEffectButton : TextureButton
{
    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        Pressed += OnButtonPressed;
    }

    private void OnMouseEntered()
    {
        SoundUiController.Instance.Hover();
        GD.Print("SingleEffectButton: Mouse Entered");
    }

    private void OnButtonPressed()
    {
        SoundUiController.Instance.Sure();
        GD.Print("SingleEffectButton: Button Pressed");
    }
}
