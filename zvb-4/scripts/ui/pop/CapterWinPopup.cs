using Godot;
using System;

public partial class CapterWinPopup : PopupPanel
{
	public static CapterWinPopup Instance { get; private set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Instance = this;
        this.Hide();
        HidePopup();
	}
    
    bool isShowing = false;

	public void ShowPopup()
    {
        if (isShowing) return;
        isShowing = true;
        Visible = true;
        this.Show();
        // 例如，显示胜利界面，停止游戏等
        SoundUiController.Instance?.Win();
        //
        SystemController.Instance.PauseGame();
    }

	public void HidePopup()
	{
        if (!isShowing) return;
        isShowing = false;
		this.Hide();
        Visible = false;
        SystemController.Instance.ResumeGame();
	}
}
