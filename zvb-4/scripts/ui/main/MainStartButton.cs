using Godot;
using System;
using ZVB4.Conf;

public partial class MainStartButton : TextureButton
{
    // 对外暴露的场景名称变量
    [Export]
    public string SceneName = "main_next.tscn";

    public static MainStartButton Instance { get; private set; }

    
    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        // TODO: 这里写点击后的逻辑
        UiTool.NextScene(this, FolderConstants.Scenes + SceneName);
    }

    // 提供方法修改场景名
    public void SetSceneName(string newName)
    {
        SceneName = newName;
    }
}
