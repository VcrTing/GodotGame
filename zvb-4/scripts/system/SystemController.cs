using Godot;
using System;
using ZVB4.Conf;

public partial class SystemController : Node2D
{
    public static SystemController Instance { get; private set; }
    
    public override void _Ready()
    {
        Instance = this;
        Init();
    }

    void Init()
    {
        DisplayServer.WindowSetSize(new Vector2I((int)GameContants.ScreenHalfW * 2, (int)GameContants.ScreenHalfH * 2));

        // 获取场景树
        var sceneTree = GetTree();

        // 固定最大帧率为90（0表示不限制）
        Engine.MaxFps = 90;
    }

    // 暂停
    public void PauseGame()
    {
        try
        {
            GetTree().Paused = true;
        }
        catch { }
    }

    // 恢复
    public void ResumeGame()
    {
        try
        {
            GetTree().Paused = false;
        }
        catch { }
    }
}
