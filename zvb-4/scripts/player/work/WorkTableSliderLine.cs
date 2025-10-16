using Godot;
using System;
using ZVB4.Conf;

public partial class WorkTableSliderLine : Node2D
{
	private Area2D _uxArea;
	private bool _isDragging = false;

	public override void _Ready()
	{
		_uxArea = GetNodeOrNull<Area2D>(NameConstants.UxArea);
		if (_uxArea != null)
		{
			_uxArea.InputEvent += OnUxAreaInputEvent;
		}
	}

    private void OnUxAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
		// 鼠标/触屏点击
		if (@event is InputEventMouseButton mouseBtn && mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			OnUxAreaClick(mouseBtn.Position);
			_isDragging = true;
		}
		else if (@event is InputEventScreenTouch touch && touch.Pressed)
		{
			OnUxAreaClick(touch.Position);
			_isDragging = true;
		}
		// 鼠标/触屏拖拽
		else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
		{
			OnUxAreaDrag(mouseMotion.Position);
		}
		else if (@event is InputEventScreenDrag screenDrag)
		{
			OnUxAreaDrag(screenDrag.Position);
		}
		// 松开时结束拖拽
		else if (@event is InputEventMouseButton mouseBtnUp && !mouseBtnUp.Pressed && mouseBtnUp.ButtonIndex == MouseButton.Left)
		{
			_isDragging = false;
		}
		else if (@event is InputEventScreenTouch touchUp && !touchUp.Pressed)
		{
			_isDragging = false;
		}
    }


	// 点击事件处理
	private void OnUxAreaClick(Vector2 pos)
	{
		GD.Print($"UxArea Click at {pos}");
		// TODO: 调用你的方法，传入坐标
	}

	// 拖拽事件处理
	private void OnUxAreaDrag(Vector2 pos)
	{
		GD.Print($"UxArea Drag at {pos}");
		// TODO: 调用你的方法，传入坐标
	}
}
