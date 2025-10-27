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
            _uxArea.MouseExited += OnUxAreaMouseExited;
		}
	}
    private void OnUxAreaMouseExited()
    {
        // 鼠标离开区域时的处理逻辑
        _isDragging = false;
    }
    float x = 0f;
    private void OnUxAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        // Viewport vp = viewport as Viewport;
        // Camera2D camera = vp.GetCamera2D();
        Vector2 src = new Vector2();

        // 鼠标/触屏点击
        if (@event is InputEventMouseButton mouseBtn && mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
        {
            // src = mouseBtn.Position;
            src = GetGlobalMousePosition();
            x = src.X;
            // OnUxAreaClick(mouseBtn.Position);
            _isDragging = true;
        }
        else if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            src = touch.Position;
            x = src.X;
            // OnUxAreaClick(touch.Position);
            _isDragging = true;
        }
        // 鼠标/触屏拖拽
        else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            src = GetGlobalMousePosition();
            x = src.X;
            // src = mouseMotion.Position;
            // OnUxAreaDrag(mouseMotion.Position);
        }
        else if (@event is InputEventScreenDrag screenDrag)
        {
            x = src.X;
            src = screenDrag.Position;
            /// OnUxAreaDrag(screenDrag.Position);
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
        // GD.Print($"WorkTableSliderLine: OnUxAreaClick at {pos}");
		// MoveSliderToPos(pos);
	}

	// 拖拽事件处理
	private void OnUxAreaDrag(Vector2 pos)
	{
		// GD.Print($"WorkTableSliderLine: OnUxAreaDrag 1  at {pos}");
		// GD.Print($"WorkTableSliderLine: OnUxAreaDrag 2 at {GetGlobalMousePosition()}");
		MoveSliderToPos(pos);
	}

    [Export]
    public float XLimit = 360f;
    public void MoveSliderToPos(Vector2 pos)
    {
        Vector2 newPos = pos;
        if (Math.Abs(newPos.X) < 0.1f)
            newPos.X = 0f;

        if (Math.Abs(newPos.X) > XLimit)
            newPos.X = XLimit * Math.Sign(newPos.X);

        PlayerController.Instance?.SlideLocation(newPos);
    }

    public override void _Process(double delta)
    {
        MoveSliderToPos(new Vector2(x, 0));
    }
}
