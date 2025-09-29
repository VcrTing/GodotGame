using Godot;
using System;
using ZVB4.Conf;

public partial class MyGameCamera : Camera2D
{
	// 设计高度
	private const float DESIGN_HEIGHT = GameContants.ScreenHalfH * 2;

	public override void _Ready()
	{
		// 获取屏幕实际高度
		float screenH = GetViewportRect().Size.Y;
		// 计算缩放比例
		float zoomY = screenH > 0 ? screenH / DESIGN_HEIGHT : 1f;
		// 设置摄像机缩放
		Zoom = new Vector2(zoomY, zoomY);
	}
}
