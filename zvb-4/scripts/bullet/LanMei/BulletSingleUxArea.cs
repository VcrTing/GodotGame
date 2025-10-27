using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class BulletSingleUxArea : Area2D
{
	private bool hasTriggered = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		try
		{
			if (area == null) return;
			Node2D pa = area.GetParent<Node2D>();
			if (pa == null) return;
			IObj obj = pa as IObj;
			if (obj != null)
			{
				EnumObjType objType = obj.GetEnumObjType();
				if (objType == EnumObjType.Zombie)
				{
					OnFirstAreaEntered(area);
				}
				else if (objType == EnumObjType.Things)
				{
					OnFirstAreaEntered(area);
				}
				else
				{
					// 不是僵尸类型，直接返回
					return;
				}
			}
		}
		catch (Exception e)
        {
            
        }
	}

	// 检测到第一个子对象时执行的方法
	private void OnFirstAreaEntered(Area2D area)
	{
		if (!hasTriggered)
		{
			hasTriggered = true;
			StartWorking();
		}
	}

	void StartWorking()
	{
		IWorking workingParent = GetParent<IWorking>();
		if (workingParent != null)
		{
			workingParent.SetWorkingMode(true);
			QueueFree();
		}
	}
}
