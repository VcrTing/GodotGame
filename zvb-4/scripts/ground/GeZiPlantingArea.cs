using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class GeZiPlantingArea : Area2D
{
	private Node2D _highlightNode;
    //
	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		AreaExited += OnAreaExited;
        _highlightNode = GetNodeOrNull<Node2D>("HightLight");
        SetHighlightVisible(false);
        lastArea = null;
        lastObj = null;
	}
    //
    private void OnAreaEntered(Area2D area)
    {
        try
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
        catch (Exception ex)
        {
            // GD.PrintErr($"GeZiPlantingArea OnAreaEntered error: {ex.Message}");
            return;
        }
    }
    //
    public void AfterDoingKilling()
    {
        PlansBaseMiao miao = lastObj as PlansBaseMiao;
        if (miao != null)
        {
            miao.ReleasePlanting();
        }
        SetHighlightVisible(false);
        lastArea = null;
        lastObj = null;
    }
    public void AfterDoingBacking()
    {
        PlansBaseMiao miao = lastObj as PlansBaseMiao;
        if (miao != null)
        {
            miao.BackToPosition();
        }
        SetHighlightVisible(false);
        lastArea = null;
        lastObj = null;
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
    //
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
    //
	public void SetHighlightVisible(bool visible)
	{
		if (_highlightNode != null)
			_highlightNode.Visible = visible;
	}
}
