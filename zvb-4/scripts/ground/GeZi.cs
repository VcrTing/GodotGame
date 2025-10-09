using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class GeZi : Node2D
{
	private Area2D _blockArea;
	GeZiPlantingArea plantingArea;


	public override void _Ready()
	{
		_blockArea = GetNodeOrNull<Area2D>(NameConstants.UxArea);
        if (_blockArea != null)
        {
            _blockArea.Connect("input_event", new Callable(this, nameof(OnBlockAreaInputEvent)));
        }
        plantingArea = GetNodeOrNull<GeZiPlantingArea>("PlantingArea");
	}

	private void OnBlockAreaInputEvent(Node viewport, InputEvent @event, int shapeIdx)
	{
		// 鼠标点击/松开
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				OnBlockAreaMouseUp();
			}
			else if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				OnBlockAreaMouseDown();
			}
		}
		// 触屏点击/松开
		else if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.Pressed)
			{
				OnBlockAreaMouseDown();
			}
			else
			{
				OnBlockAreaMouseUp();
			}
		}
	}

    void RunningPlanting(Area2D area, IObj obj)
    {
        PlansPlantingArea ppa = area as PlansPlantingArea;
        if (ppa != null)
        {
            Area2D me = ppa.GetLastEnteredArea();
            if (me == plantingArea) {
                // GD.Print("锁定你这个格子");
                // 尝试种植

                // 
            }
        }
    }

    private void OnBlockAreaMouseUp()
    {
        // TODO: 实现松开时的逻辑
        if (plantingArea != null)
        {
            IObj obj = plantingArea.lastObj;
            if (obj != null)
            {
                RunningPlanting(plantingArea.lastArea, obj);
            }
        }
    }

    private void OnBlockAreaMouseDown()
    {
        GD.Print("GeZi: Area2D内鼠标按下");
        // TODO: 实现点击时的逻辑
    }
    
	// 维护Node2D对象列表
	private System.Collections.Generic.List<Node2D> _node2DList = new System.Collections.Generic.List<Node2D>();

    /// <summary>
    /// 尝试将Node2D加入列表，若已存在则不加，加入时可做复杂判断（此处默认true）
    /// </summary>
    public bool TryAddNode2D(Node2D node)
    {
        if (node == null) return false;
        if (_node2DList.Contains(node)) return false;
        // 复杂判断逻辑，现默认true
        bool canAdd = IsAllowJoin(node);
        if (canAdd)
        {
            _node2DList.Add(node);
            return true;
        }
        return false;
    }
    
	/// <summary>
	/// 移除Node2D对象，若存在则移除
	/// </summary>
	public bool RemoveNode2D(Node2D node)
	{
		if (node == null) return false;
		return _node2DList.Remove(node);
	}

	public bool IsAllowJoin(Node2D node)
	{
		if (node == null) return false;
        IObj youobj = node as IObj;
        if (youobj == null) return false;
        string name = youobj.GetObjName();
        string nameSame = "";
        // MMM: 检查列表中是否有同名的IObj
        foreach (var n in _node2DList)
        {
            IObj other = n as IObj;
            if (other != null && other.GetObjName() == name)
            {
                nameSame = other.GetObjName();
                break;
            }
        }
        // 相同的坚果，阔以种下
        if (nameSame == PlansConstants.JianGuo)
        {
            return true;
        }
        // 有其他同名的植物，不允许种下
        if (nameSame != "")
        {
            return false;
        }
        return true;
	}
}
