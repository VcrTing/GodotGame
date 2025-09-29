using Godot;
using System;
using ZVB4.Conf;

public partial class MainStartButton : TextureButton
{
    // 对外暴露的场景名称变量
    [Export]
    public string SceneName = "main_next.tscn";

    public static MainStartButton Instance { get; private set; }

    [Export]
    public float DelayDoing = 0.2f; // 延迟时间，单位为秒
    
    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
    }

    private async void OnButtonPressed()
    {
        GD.Print("MainStartButton Pressed!");
        // TODO: 这里写点击后的逻辑

        SoundUiController.Instance.Sure();

        // 0.2s后切换下一个场景
        await ToSignal(GetTree().CreateTimer(DelayDoing), "timeout");
        GetTree().ChangeSceneToFile(FolderConstants.Scenes + SceneName);
    }

    // 提供方法修改场景名
    public void SetSceneName(string newName)
    {
        SceneName = newName;
    }
}
