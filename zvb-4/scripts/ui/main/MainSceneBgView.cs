using Godot;
using System;

public partial class MainSceneBgView : AnimatedSprite2D
{
    public override void _Ready()
    {
        //
        GodotTool.GetViewAndAutoPlay(this);
    }

}
