using Godot;
using System;

public partial class ThingsFinishedDoor : Area2D
{
    [Export]
    public string NextScenePath { get; set; } = "";


    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayerBody player)
        {
            // 玩家触碰到终点门时，触发游戏胜利逻辑
            GD.Print("游戏胜利！");
            // 这里可以添加更多的胜利处理逻辑，例如显示胜利界面等
            NextScene();
        }
    }

    void NextScene()
    {
        if (!string.IsNullOrEmpty(NextScenePath))
        {
            GD.Print("NextScenePath: " + NextScenePath);
            GetTree().ChangeSceneToFile("res://guanka/" + NextScenePath + ".tscn");
        }
    }
}
