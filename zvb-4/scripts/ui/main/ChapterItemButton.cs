using Godot;
using System;
using ZVB4.Conf;

public partial class ChapterItemButton : TextureButton
{
    // 对外暴露的场景名称变量
    [Export]
    public EnumChapter Chapter = EnumChapter.One1;
    [Export]
    public string ChapterTitle = "";
    [Export]
    public string ChapterDesc = "";

    private Label _labelTitle;
    private Label _labelDesc;

	public static ChapterItemButton Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
        _labelTitle = GodotTool.FindLabelByName(this, "LabelTitle");
        _labelDesc = GodotTool.FindLabelByName(this, "LabelDesc");
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
        // string scenePath = FolderConstants.Scenes + ChapterTool.GetChapterSceneName(_cap);
        // GetTree().ChangeSceneToFile(scenePath);
        UiTool.OpenBaseShooterOrStartGame(this, _cap);
        // BaseShooterSwitcherPopup.Instance.ShowPopup();
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
        if (_labelTitle != null)
        {
            if (ChapterTitle != "")
            {
                _labelTitle.Text = ChapterTitle;
            }
            else
            {
                _labelTitle.Text = $"{_cap}";
            }
        }
        if (_labelDesc != null)
        {
            if (ChapterDesc != "")
            {
                _labelDesc.Text = ChapterDesc;
            }
            else
            {
                
            }
        }
    }

}
