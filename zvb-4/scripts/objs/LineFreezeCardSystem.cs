using Godot;
using System;
using ZVB4.Conf;

public partial class LineFreezeCardSystem : Node2D
{
    public static LineFreezeCardSystem Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public Node2D CreateFreezeCard(Godot.Collections.Dictionary buffInfo)
    {
        
        //
        string planName = buffInfo["plansname"].AsString();
        float viewScale = buffInfo["viewscale"].AsSingle();
        int amount = buffInfo["amount"].AsInt32();
        //
        int typedidx = buffInfo["type"].AsInt32();
        EnumRewords rewordType = (EnumRewords)typedidx;
        //
        int buffidx = buffInfo["buff"].AsInt32();
        EnumPlayerBuff buff = (EnumPlayerBuff)buffidx;
        //
        int hp = buffInfo["hp"].AsInt32();
        float x = buffInfo["x"].AsSingle();
        float y = buffInfo["y"].AsSingle();
        
        // 实例化LineFreezeCard  
        var scene = GD.Load<PackedScene>(FolderConstants.WaveThings + "line_freeze_card.tscn");
        if (scene == null) return null;
        var card = scene.Instantiate<Node2D>();
        AddChild(card);
        if (card is LineFreezeCard freezeCard)
        {
            freezeCard.Init(x, y, rewordType, amount, viewScale, hp, planName, buff);
        }
        return card;
    }
}
