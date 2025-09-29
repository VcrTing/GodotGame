using Godot;
using System;

public partial class LimitAreaPlayerTouch : Node2D
{
	public override void _Ready()
	{
		// 假设子节点名为"Area2D"
		Area2D area = GetNode<Area2D>("Area2D");
		area.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is PlayerBody)
		{
			// 重置游戏，重新加载当前场景
			GetTree().ReloadCurrentScene();
		}
	}
}
