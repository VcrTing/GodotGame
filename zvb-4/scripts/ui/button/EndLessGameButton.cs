using Godot;
using System;
using ZVB4.Conf;

public partial class EndLessGameButton : TextureButton
{
    [Export]
    public string EndLessKey;
    [Export]
    public bool IsOpenNewGame;

    public static EndLessGameButton Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        this.Pressed += OnButtonPressed;
    }

    async void OpenNewGame()
    {
        string ph = "";
        if (EndLessKey == EndLessTool.EndLessKeyDay)
        {
            ph = FolderConstants.Scenes + "chapter_9endless.tscn";
        }
        else if (EndLessKey == EndLessTool.EndLessKeyNight)
        {
            ph = FolderConstants.Scenes + "chapter_9endless.tscn";
        }
        UiTool.NextScene(this, ph);
    }
    //
    private void OnButtonPressed()
    {
        if (IsOpenNewGame)
        {
            OpenNewGame();
        }
        else
        {
            GD.Print("读取白日存档。");
        }
    }
}
