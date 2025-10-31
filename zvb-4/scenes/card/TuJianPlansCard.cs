using Godot;
using System;
using ZVB4.Conf;

public partial class TuJianPlansCard : HBoxContainer
{
    [Export]
    public string PlansName = "";
    [Export]
    public float SceneScale = 1f;
    [Export]
    public float SceneLocX = 120f;
    [Export]
    public float SceneLocY = 120f;
    [Export]
    public float LazyLoadTime = 0f;

	public override void _Ready()
    {
		InitCard();
	}
    string CardDir = FolderConstants.Designs;


    public async void InitCard()
    {
        await ToSignal(GetTree().CreateTimer(LazyLoadTime), "timeout");
        I18nConstants.ShowTuJianPlansCard(this, PlansName, SceneLocX, SceneLocY, 1f);
    }
}
