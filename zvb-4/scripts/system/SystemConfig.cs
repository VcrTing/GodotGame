using Godot;
using System;
using ZVB4.Conf;

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
        DisplayServer.WindowSetSize(new Vector2I((int)GameContants.ScreenHalfW * 2, (int)GameContants.ScreenHalfH * 2));
    }
}
