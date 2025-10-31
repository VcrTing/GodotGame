using Godot;
using System;
using System.Diagnostics;

public partial class PopOptionButton : TextureButton
{
    [Export]
    public string k = "store_ingame";

    [Export]
    public bool IsClose = true;
    
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
        switch (k)
        {
            case "store_ingame":
                if (IsClose)
                {
                    if (StoreInGamePopup.Instance.IsVisible())
                    {
                        StoreInGamePopup.Instance.HidePopup();
                        return;
                    }
                }
                else
                {
                    StoreInGamePopup.Instance.ShowPopup();
                }
                break;
            case "base_shooter":
                if (IsClose)
                {
                    if (BaseShooterSwitcherPopup.Instance.IsVisible())
                    {
                        BaseShooterSwitcherPopup.Instance.HidePopup();
                        return;
                    }
                }
                else
                {
                    BaseShooterSwitcherPopup.Instance.ShowPopup();
                }
                break;
            default:
                GD.Print($"PopOptionButton: Unknown key {k}");
                break;
        }
    }
}
