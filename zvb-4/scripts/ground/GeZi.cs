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

    void RunningPlanting(IObj obj)
    {
        bool ok = false;
        string n = obj.GetObjName();
        if (n != "")
        {
            string allowName = AllowPlans(obj as Node2D);
            if (allowName != "")
            {
                // 射手就销毁
                if (PlansConstants.IsShooter(allowName))
                {
                    DieToReword(plantingArea.lastObj as Node2D);
                    ok = true;
                }
                // 种植
                else {
                    Node2D plantNode = ReplaceOldThenNewOne(allowName);
                    if (plantNode != null)
                    {
                        ok = true;
                        // 非消费型植物才加入列表
                        if (PlansConstants.IsWillZhanYongGeZi(allowName))
                        {
                            AddNode2DOnlyOne(plantNode as Node2D);
                        }
                        // 播放种植音效
                        SoundFxController.Instance?.PlayFx("Ux/zhongxia", "ZhongXia", 4);
                    }
                }
            }
        }
        //
        if (ok)
        {
            plantingArea.AfterDoingKilling();
        }
        else
        {
            plantingArea.AfterDoingBacking();
        }
    }

    Node2D ReplaceOldThenNewOne(string n) {
        Node2D node = null;
        if (_node2DList.Count == 0)
        {
            node = ZhongXiaPlans(n);
        }
        else {
            // 删掉旧的
            foreach (var nd in _node2DList)
            {
                string oldName = (nd as IObj)?.GetObjName() ?? "";
                if (oldName == n) {
                    _node2DList.Remove(nd);
                    nd.QueueFree();
                    node = ZhongXiaPlans(n);
                    break;
                }
            }
        }
        return node;
    }

    Node2D ZhongXiaPlans(string n) {
        // 开始种植
        string scenePath = PlansConstants.GetPlanScene(n);
        if (!string.IsNullOrEmpty(scenePath))
        {
            var scene = GD.Load<PackedScene>(scenePath);
            if (scene != null)
            {
                Node2D pls = (Node2D)scene.Instantiate();
                // 拿到格子中心点
                AddChild(pls);
                // 设置工作模式
                if (pls is IWorking working)
                {
                    working.SetWorkingMode(true);
                    return pls;
                }
            }
        }
        return null;
    }

    private void OnBlockAreaMouseUp()
    {
        // TODO: 实现松开时的逻辑
        if (plantingArea != null)
        {
            IObj obj = plantingArea.lastObj;
            if (obj != null)
            {
                PlansPlantingArea ppa = plantingArea.lastArea as PlansPlantingArea;
                if (ppa != null)
                {
                    Area2D me = ppa.GetLastEnteredArea();
                    if (me == plantingArea)
                    {
                        RunningPlanting(obj);
                    }
                }
            }
        }
    }

    private void OnBlockAreaMouseDown()
    {
        // GD.Print("GeZi: Area2D内鼠标按下");
        // TODO: 实现点击时的逻辑
    }
    
	// 维护Node2D对象列表
	private System.Collections.Generic.List<Node2D> _node2DList = new System.Collections.Generic.List<Node2D>();

    /// <summary>
    /// 尝试将Node2D加入列表，若已存在则不加，加入时可做复杂判断（此处默认true）
    /// </summary>
    public bool AddNode2D(Node2D node)
    {
        if (node == null) return false;
        if (_node2DList.Contains(node)) return false;
        _node2DList.Add(node);
        return true;
    }
    public bool AddNode2DOnlyOne(Node2D node)
    {
        if (node == null) return false;
        if (_node2DList.Contains(node)) return false;
        _node2DList.Clear();
        _node2DList.Add(node);
        return true;
    }
    /// <summary>
    /// 移除Node2D对象，若存在则移除
    /// </summary>
    public bool RemoveNode2D(Node2D node)
    {
        if (node == null) return false;
        return _node2DList.Remove(node);
    }

    string AllowPlans(Node2D node) {
        string name = CommonTool.GetNameOfNode2D(node);
        // 无植物
        if (_node2DList.Count == 0) return name;
        string ns = "";
        // MMM: 检查列表中是否有同名的IObj
        foreach (var n in _node2DList)
        {
            IObj other = n as IObj;
            if (other != null && other.GetObjName() == name)
            {
                ns = other.GetObjName();
                break;
            }
        }
        // 相同的坚果，阔以种下
        if (ns == PlansConstants.JianGuo)
        {
            return ns;
        }
        return "";
    }
    public bool IsAllowJoin(Node2D node) => (AllowPlans(node) != "");

    // 解锁格子
    public bool UnLockGezi(IObj obj)
    {
        if (obj == null) return false;
        string n = obj.GetObjName();    
        Node2D nd = CommonTool.LocationNode2DByName(_node2DList, n);
        if (nd != null)
        {
            _node2DList.Remove(nd);
            return true;
        }
        return false;
    }
    
    // 死亡
    public void DieToReword(Node2D lastNode)
    {
        // 生成奖励组
        try
        {
            var rewordGroupScene = GD.Load<PackedScene>(FolderConstants.WaveObj + "reword_group.tscn");
            var rewordGroupNode = rewordGroupScene.Instantiate<Node2D>();
            GetParent().AddChild(rewordGroupNode);
            var rewordGroup = rewordGroupNode as RewordGroup;
            if (rewordGroup != null)
            {
                Vector2 pos = lastNode != null ? lastNode.Position : this.Position;
                IObj lastObj = lastNode as IObj;
                // 阳光数量
                int num = SunMoneyConstants.GetSunNumNormal(lastObj.GetObjName());
                // 生成阳光奖励
                rewordGroup.SpawnReword(SunMoneyConstants.Sun, num, pos, SunMoneyConstants.SunNormal);
                // 
                SoundFxController.Instance?.PlayFx("Ux/trash", "trash", 4, pos);
            }

            // 销毁苗
        }
        catch
        {
            
        }
    }  
}
