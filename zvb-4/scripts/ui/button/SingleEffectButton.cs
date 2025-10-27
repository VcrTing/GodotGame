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
    }

    private void OnButtonPressed()
    {
        SoundUiController.Instance.Sure();
    }
}
