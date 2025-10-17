using Godot;
using System;

public partial class GameModeSwitchCenter : Node2D
{
    public static GameModeSwitchCenter Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        LoadGame();
    }

    void LoadGame()
    {
        var ins = SaveGamerRunnerDataManger.Instance;
        if (ins == null)
        {
            GetTree().CreateTimer(0.1f).Timeout += () => LoadGame();
            return;
        }
        int chapterNumber = ins.GetCapterNumber();
        string scenePath = ChapterTool.GetChapterSystemLoader(chapterNumber);
        if (scenePath != "")
        {
            // NNN: 生成scenePath的实例并加入场景树
            var scene = GD.Load<PackedScene>(scenePath);
            if (scene != null)
            {
                var instance = scene.Instantiate<Node2D>();
                AddChild(instance);
            }
        }
    }
}
