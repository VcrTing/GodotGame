using Godot;
using System;
using ZVB4.Conf;

public partial class EnmyZeroView : Node2D
{
	public override void _Ready()
	{
		var sprite = GetNode<AnimatedSprite2D>(NameConstants.Sprite);
		if (sprite != null)
		{
			sprite.Play("walk");
		}
	}
}
