using Godot;
using System;

public partial class StoreInGamePopup : PopupPanel
{
    public static StoreInGamePopup Instance { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
        this.PopupHide += WhenHide;
    }

    public bool IsShowing()
    {
        return Visible;
    }

    public void ShowPopup()
    {
        SystemController.Instance?.PauseGame();
        SoundUiController.Instance.ShopStart();
        Show();
    }

    void WhenHide()
    {
        SystemController.Instance?.ResumeGame();
    }

    public void HidePopup()
    {
        WhenHide();
        Hide();
    }
}
