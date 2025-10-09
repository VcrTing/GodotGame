using Godot;
using System;

public partial class PlansPlantingArea : Area2D
{
	public Area2D LastEnteredArea { get; private set; }

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

    private void OnAreaEntered(Area2D area)
    {
        LastEnteredArea = area;
        GD.Print($"PlansPlantingArea: Area entered: {area?.Name}");
    }
    
    public Area2D GetLastEnteredArea()
    {
        return LastEnteredArea;
    }
}
