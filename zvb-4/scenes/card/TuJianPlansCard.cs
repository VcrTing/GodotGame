using Godot;
using System;
using ZVB4.Conf;

public partial class TuJianPlansCard : HBoxContainer
{
    [Export]
    public string CardPath = "tujian/plansshooter/Pea.json";
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
        string CardPath = CardDir + this.CardPath;
        // 读取卡片数据（JSON -> Dictionary）
        if (Godot.FileAccess.FileExists(CardPath))
        {
            using var file = Godot.FileAccess.Open(CardPath, Godot.FileAccess.ModeFlags.Read);
            var jsonText = file.GetAsText();
            var json = new Json();
            var err = json.Parse(jsonText);
            if (err == Error.Ok)
            {
                var cardData = json.Data.AsGodotDictionary();
                if (cardData != null)
                {
                    var loader = GodotTool.FindCanvasItemByName(this, "TextureLoader");
                    if (loader != null && cardData.ContainsKey("scenepath"))
                    {
                        var scenePath = FolderConstants.WaveHouse + cardData["scenepath"].AsString();

                        if (!string.IsNullOrEmpty(scenePath))
                        {
                            var packedScene = GD.Load<PackedScene>(scenePath);
                            if (packedScene != null)
                            {
                                var instance = packedScene.Instantiate();
                                if (instance is Node2D node2D)
                                {
                                    loader.AddChild(node2D);
                                    // node2D.Scale = new Vector2(SceneScale, SceneScale);
                                    node2D.Position = new Vector2(SceneLocX, SceneLocY);
                                }
                            }
                        }
                    }
                    var nameLabel = GodotTool.FindLabelByName(this, "LabelName");
                    if (nameLabel != null && cardData.ContainsKey("nickname")) nameLabel.Text = cardData["nickname"].AsString();
                    var infoLabel = GodotTool.FindLabelByName(this, "LabelInfo");
                    if (infoLabel != null && cardData.ContainsKey("info")) infoLabel.Text = cardData["info"].AsString();
                    var descLabel = GodotTool.FindLabelByName(this, "LabelDesc");
                    if (descLabel != null && cardData.ContainsKey("description")) descLabel.Text = cardData["description"].AsString();
                }
            }
        }
	}
}
