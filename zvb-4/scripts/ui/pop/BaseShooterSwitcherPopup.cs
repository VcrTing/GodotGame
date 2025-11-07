using Godot;
using System;
using ZVB4.Conf;

public partial class BaseShooterSwitcherPopup : PopupPanel
{
	public static BaseShooterSwitcherPopup Instance { get; private set; }


    TextureButton startGameButton;
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
        this.PopupHide += WhenHide;
        startGameButton = GodotTool.FindCanvasItemByName(this, "StartGameButton") as TextureButton;
        if (startGameButton != null)
        {
            startGameButton.Pressed += OnButtonPress;
        }
    }

    async void OnButtonPress()
    {
        SoundUiController.Instance.GameStart();
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        int _cap = SaveGamerRunnerDataManger.Instance.GetCapterNumber();
        string scenePath = FolderConstants.Scenes + ChapterTool.GetChapterSceneName(_cap);
        GetTree().ChangeSceneToFile(scenePath);
    }

    public bool IsShowing()
    {
        return Visible;
    }

    public void ShowPopup()
    {
        // SystemController.Instance?.PauseGame();
        SoundUiController.Instance.ShopStart();
        Show();
    }

    void WhenHide()
    {
        // SystemController.Instance?.ResumeGame();
    }

    public void HidePopup()
    {
        WhenHide();
        Hide();
    }
}
