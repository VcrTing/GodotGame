using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class BulletXiguaBingUxArea : Area2D
{
	private bool hasTriggered = false;
    IBulletBase bulletBase;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
		PlayFlyFx();
        bulletBase = GetParent<IBulletBase>();
	}

	async void PlayFlyFx()
	{
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		Vector2 vector2 = bulletBase.GetDirection();
		float x = vector2.X * 20;
		SoundFxController.Instance?.PlayFx("Fx/XiguaBing", "XiguaBing_fly", 4, new Vector2(x, 0));
	}

	private void OnAreaEntered(Area2D area)
	{
		IObj obj = area.GetParent<IObj>();
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
