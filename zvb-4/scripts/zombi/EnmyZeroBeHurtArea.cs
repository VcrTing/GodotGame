using Godot;
using System;

public partial class EnmyZeroBeHurtArea : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        GD.Print("EnmyZeroBeHurtArea: Body init");
    }

    private void OnBodyEntered(Node body)
    {
        if (body != null)
        {
            GD.Print($"EnmyZeroBeHurtArea detected: {body.Name}");
            // 这里可以添加进一步的逻辑
        }
        else
        {
            GD.Print("EnmyZeroBeHurtArea detected: null");
        }
    }
}
