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
        ProcessMode = ProcessModeEnum.Always;
        this.MouseEntered += () => SoundUiController.Instance.Hover();
        // 获取Label子节点并设置文字
        var label = GetNodeOrNull<Label>(NameConstants.Label);
        if (label != null)
        {
            label.Text = LabelText;
        }

        if (ButtonType == EnumOptions.NextChapterAuto)
        {
            
        }
    }

    private void OnButtonPressed()
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
            case EnumOptions.PauseGame:
                GameStopPopup.Instance.Change(true);
                break;
            case EnumOptions.ResumeGame:
                GameStopPopup.Instance.Change(false);
                break;
            case EnumOptions.Queue:
                // 关闭游戏
                GetTree().Quit();
                break;
            default:
                GD.PrintErr("SingleOptionsButton: Unknown ButtonType " + ButtonType);
                break;
        }
        SystemController.Instance.ResumeGame();
    }

    void PrevScene()
    {
        string prev = FolderConstants.Pages + PrevSceneName;
        // 检查prev存在？
        if (!ResourceLoader.Exists(prev))
        {
            string prevm = FolderConstants.Scenes + PrevSceneName;
            if (ResourceLoader.Exists(prevm))
            { prev = prevm; }
        }
        GD.Print("PrevScene: Load " + prev);
        UiTool.PrevScene(this, prev);
    }

    void NextScene()
    {
        string next = FolderConstants.Pages + NextSceneName;
        // 检查next存在？
        if (!ResourceLoader.Exists(next))
        {
            string nextm = FolderConstants.Scenes + NextSceneName;
            if (ResourceLoader.Exists(nextm))
            { next = nextm; }
        }
        // GD.Print("NextScene: Load " + next);
        UiTool.NextScene(this, next);
    }

    void ReloadScene()
    {
        GetTree().ReloadCurrentScene();
    }

    async void NextChapterAuto()
    {
        await this.ToSignal(this.GetTree().CreateTimer(3f), "timeout");
        NextChapter(false);
    }

    void NextChapter(bool isBackToMain = true)
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
            if (isBackToMain) UiTool.BackToMainScene(this);
            return;
        }
        UiTool.NextScene(this, FolderConstants.Scenes + ChapterTool.GetChapterSceneName(nextCap));
    }
}
