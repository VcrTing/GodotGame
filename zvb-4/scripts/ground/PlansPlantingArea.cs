using Godot;
using System;

public partial class PlansPlantingArea : Area2D
{
	public Area2D LastEnteredGeZi { get; private set; }

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

    private void OnAreaEntered(Area2D area)
    {
        LastEnteredGeZi = area;
    }

    public Area2D GetLastEnteredArea()
    {
        return LastEnteredGeZi;
    }
    
    public void UnLockGezi() {
        if (LastEnteredGeZi != null) {
            
        }
        LastEnteredGeZi = null;
    }

    public void ReleasePlanting() 
    {
        // LastEnteredGeZi = null;
        // GD.Print("PlansPlantingArea: Released planting area.");
        
    }
}
