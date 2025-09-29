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
        _label = GetNodeOrNull<Label>("Label");
        UpdateLabelText();
    }

    private async void OnButtonPressed()
    {
        GD.Print("ChapterItemButton Pressed!");
        SoundUiController.Instance.Sure();
        await ToSignal(GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
        // 加载章节场景
        int _cap = (int)Chapter;
        string _caps = _cap.ToString();
        // 第一章
        if (_caps.StartsWith("1"))
        {
            string scenePath = FolderConstants.Scenes + "test.tscn";
            GetTree().ChangeSceneToFile(scenePath);
        }
	}

    public void SetChapter(EnumChapter chapter)
    {
        Chapter = chapter;
        UpdateLabelText();
    }

    private void UpdateLabelText()
    {
        int _cap = (int)Chapter;
        string displayNum = _cap >= 100 ? _cap.ToString().Substring(1) : _cap.ToString();
        if (_label != null)
        {
            _label.Text = $"第{displayNum}关";
        }
    }

}
