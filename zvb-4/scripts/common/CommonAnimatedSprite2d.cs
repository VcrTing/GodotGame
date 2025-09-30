using Godot;
using System;
using ZVB4.Conf;

public partial class CommonAnimatedSprite2d : AnimatedSprite2D
{
	public override void _Ready()
	{
		Play(NameConstants.Default);
	}
}
