using Godot;
using System;
using ZVB4.Conf;

public partial class ShooterView : Node2D
{
	public override void _Ready()
	{
		var view2D = GetNode<AnimatedSprite2D>(NameConstants.View);
		if (view2D != null)
		{
			view2D.Play(NameConstants.Default);
		}
	}
}
