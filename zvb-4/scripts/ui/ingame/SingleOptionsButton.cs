using Godot;
using System;
using ZVB4.Conf;

public partial class SingleOptionsButton : TextureButton
{
    [Export]
    public string PrevSceneName = ChapterTool.MainMenuScene;
    [Export]
    public string NextSceneName = ChapterTool.MainNextScene;
    [Export]
    public string LabelText = "Customer Text";
    [Export]
    public EnumOptions ButtonType = EnumOptions.NextScene;


    public static SingleOptionsButton Instance { get; private set; }


    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
        // 获取Label子节点并设置文字
        var label = GetNodeOrNull<Label>(NameConstants.Label);
        if (label != null)
        {
            label.Text = LabelText;
        }
    }

    private async void OnButtonPressed()
    {
        switch (ButtonType)
        {
            case EnumOptions.NextChapter:
                NextChapter();
                break;
            case EnumOptions.RePlayScene:
                ReloadScene();
                break;
            case EnumOptions.NextScene:
                NextScene();
                break;
            case EnumOptions.Back:
                PrevScene();
                break;
            case EnumOptions.BackToMain:
                UiTool.BackToMainScene(this);
                break;
            default:
                GD.PrintErr("SingleOptionsButton: Unknown ButtonType " + ButtonType);
                break;
        }
    }

    void PrevScene()
    {
        UiTool.PrevScene(this, FolderConstants.Scenes + PrevSceneName);
    }

    void NextScene()
    {
        UiTool.NextScene(this, FolderConstants.Scenes + NextSceneName);
    }

    void ReloadScene()
    {
        GetTree().ReloadCurrentScene();
    }

    void NextChapter()
    {
        // TODO: 这里写点击后的逻辑
        var manager = SaveGamerRunnerDataManger.Instance;
        if (manager == null)
        {
            GD.PrintErr("SaveGamerRunnerDataManger Instance is null!");
            return;
        }
        int nextCap = manager.DoNextChapter();
        if (nextCap == (int)EnumChapter.None)
        {
            UiTool.BackToMainScene(this);
            return;
        }
        UiTool.NextScene(this, FolderConstants.Scenes + ChapterTool.GetNextChapterSceneName(nextCap));
    }
}
