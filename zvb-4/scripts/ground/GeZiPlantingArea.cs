using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class GeZiPlantingArea : Area2D
{
	private Node2D _highlightNode;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
        _highlightNode = GetNodeOrNull<Node2D>("HightLight");
        SetHighlightVisible(false);
	}

    private void OnAreaEntered(Area2D area)
    {
        lastArea = null; 
        lastObj = null;
        IWorking iwk = area.GetParent<IWorking>();
        if (iwk != null)
        {
            if (iwk.IsWorking())
            {
                SetHighlightVisible(true);
                IObj obj = area.GetParent<IObj>();
                if (obj != null) 
                {
                    EnterGrass(area, obj);
                }
            }
        }
    }
    public Area2D lastArea;
    public IObj lastObj;
    void EnterGrass(Area2D area, IObj obj)
    {
        lastArea = area; lastObj = obj;
    }
    void LeaveGrass(Area2D area, IObj obj)
    {
        lastArea = null; lastObj = null;
    }

    private void OnAreaExited(Area2D area)
    {
        IWorking iwk = area.GetParent<IWorking>();
        if (iwk != null)
        {
            if (iwk.IsWorking())
            {
                SetHighlightVisible(false);
                IObj obj = area.GetParent<IObj>();
                if (obj != null) 
                {
                    LeaveGrass(area, obj);
                }
            }
        }
    }
    
	/// <summary>
	/// 控制 HightLight 子节点的显示或隐藏
	/// </summary>
	public void SetHighlightVisible(bool visible)
	{
		if (_highlightNode != null)
			_highlightNode.Visible = visible;
	}
}
