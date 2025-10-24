using Godot;
using System;

public partial class StoreInGamePopup : PopupPanel
{
    public static StoreInGamePopup Instance { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
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

    public void HidePopup()
    {
        SystemController.Instance?.ResumeGame();
        Hide();
    }
}
