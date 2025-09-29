using Godot;
using System;

public partial class MainFollower : Node2D
{

    string __PLAYER_NAME = "PlayerBody";
	private Node2D player;

	public override void _Ready()
	{
		player = GetParent().GetNode<Node2D>(__PLAYER_NAME);
	}

	public override void _Process(double delta)
	{
		if (player != null)
		{
			Position = player.Position;
		}
	}
}
