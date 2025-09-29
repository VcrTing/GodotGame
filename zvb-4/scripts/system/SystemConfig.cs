using Godot;
using System;

public partial class SystemConfig : Node2D
{
    public static SystemConfig Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
        Init();
    }
    
    void Init()
    {
        DisplayServer.WindowSetSize(new Vector2I(1080, 1920));
    }
}
