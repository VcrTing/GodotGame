using Godot;
using System;

public partial class GameStopPopup : PopupPanel
{
	public static GameStopPopup Instance { get; private set; }
	public override void _Ready()
	{
        Instance = this;
        Change(false);
        ProcessMode = ProcessModeEnum.Always;
        // 设置为模态弹窗，禁止外部点击关闭
        this.Exclusive = true; // Godot 4.x: PopupPanel.Exclusive 禁止外部点击关闭
	}
    
    public void Change(bool isOpen)
    {
        if (isOpen)
        {
            Visible = true;
            Show();
            SystemController.Instance?.PauseGame();
            SoundUiController.Instance.Pause();
        }
        else
        {
            Hide();
            Visible = false;
            SystemController.Instance?.ResumeGame();
        }
    }
}
