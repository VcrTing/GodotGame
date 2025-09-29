using Godot;
using System;

public partial class UiSunCounter : Node2D
{
	private Label _label;
	private int _sunCount = 0;

	public static UiSunCounter Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		_label = GetNode<Label>("Label");
		UpdateLabel();

		// 获取主摄像机
		var camera = GetViewport().GetCamera2D();
		Vector2 screenTopLeft = Vector2.Zero;
		if (camera != null)
			screenTopLeft = camera.GetScreenCenterPosition() - (GetViewport().GetVisibleRect().Size / 2) / camera.Zoom;

		// 设置Label到左上角一点点
		if (_label != null)
			_label.GlobalPosition = screenTopLeft + new Vector2(10, 10);
	}

	public void AddSun(int amount)
	{
		_sunCount += amount;
		UpdateLabel();
	}

	public void SetSun(int amount)
	{
		_sunCount = amount;
		UpdateLabel();
	}

	public int GetSun()
	{
		return _sunCount;
	}

	private void UpdateLabel()
	{
		if (_label != null)
			_label.Text = $"阳光：{_sunCount}";
	}
}
