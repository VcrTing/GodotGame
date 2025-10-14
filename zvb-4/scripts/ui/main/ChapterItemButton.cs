using Godot;
using System;
using ZVB4.Conf;

public partial class ChapterItemButton : TextureButton
{
    // 对外暴露的场景名称变量
    [Export]
    public EnumChapter Chapter = EnumChapter.One1;

    private Label _label;

	public static ChapterItemButton Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
        _label = GetNodeOrNull<Label>(NameConstants.Label);
        UpdateLabelText();
    }

    private async void OnButtonPressed()
    {
        SoundUiController.Instance.Sure();
        await ToSignal(GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
        // 加载章节场景
        int _cap = (int)Chapter;
        // 保存场景
        var manager = SaveGamerRunnerDataManger.Instance;
        if (manager == null) return;
        manager.SetCapterNumber(_cap);
        string scenePath = FolderConstants.Scenes + ChapterTool.GetChapterSceneName(_cap);
        GetTree().ChangeSceneToFile(scenePath);
    }

    public void SetChapter(EnumChapter chapter)
    {
        Chapter = chapter;
        UpdateLabelText();
    }

    private void UpdateLabelText()
    {
        int i = (int)Chapter;
        string _cap = i.ToString();
        // string displayNum = _cap.Substring(1, _cap.Length - 1);
        if (_label != null)
        {
            _label.Text = $"{_cap}";
        }
    }

}
