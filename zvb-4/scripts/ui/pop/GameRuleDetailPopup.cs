using Godot;
using System;

public partial class GameRuleDetailPopup : PopupPanel
{
	public static GameRuleDetailPopup Instance { get; private set; }

	private AnimationPlayer _animPlayer;

	public override void _Ready()
	{
        Visible = true;
        Instance = this;
        Close();
	}

    public void Change(bool isOpen)
    {
        if (isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    /// <summary>
    /// 打开弹框（带淡入动画）
    /// </summary>
    public void Open()
    {
        Show();
    }

	/// <summary>
	/// 关闭弹框
	/// </summary>
	public void Close()
	{
		Hide();
	}
}
