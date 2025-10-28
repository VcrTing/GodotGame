using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class StoreLoadCardSystem : BoxContainer
{
    
    [Export]
    public string CardsJsonPath;

    List<string> excludeItems = new List<string>();

    List<Godot.Collections.Dictionary> availableItems = new List<Godot.Collections.Dictionary>();

    public override void _Ready()
    {
        LoadAndFilterItems();
    }

    float __t = 0f;
    float __delay = 1f;
    public override void _Process(double delta)
    {
        /*
        __t += (float)delta;
        if (__t >= __delay)
        {
            __t = 0f;
            if (StoreInGamePopup.Instance != null)
            {
                if (__delay < 2f || !StoreInGamePopup.Instance.IsVisible())
                {
                    LoadAndFilterItems();
                }
                __delay = GD.RandRange(1, 10) * 0.3f + 10f;
            }
        }
        */
    }

    //
    public void LoadAndFilterItems()
    {
        GD.Print("A");
        availableItems.Clear();
        if (string.IsNullOrEmpty(CardsJsonPath)) return;
        string ph = FolderConstants.Designs + "store/" + CardsJsonPath;
        GD.Print(ph);
        var file = FileAccess.Open(ph, FileAccess.ModeFlags.Read);
        if (file == null) return;
        var json = file.GetAsText();
        file.Close();
        Godot.Collections.Array data = (Godot.Collections.Array)Godot.Json.ParseString(json);
        if (data == null) return;
        foreach (var obj in data)
        {
            var dict = (Godot.Collections.Dictionary)obj;
            if (dict == null) continue;
            if (dict.ContainsKey("name") && excludeItems.Contains(dict["name"].ToString()))
                continue;
            availableItems.Add(dict);
        }
            GD.Print(availableItems);
        //
        GenerateStoreCards();
    }

    // 批量生成卡片节点
    public void GenerateStoreCards()
    {
            GD.Print("GenerateStoreCards");
        if (availableItems.Count == 0) return;
        // 先删掉自己所有的子节点
        foreach (var child in GetChildren())
        {
            if (child is Node node)
            {
                node.QueueFree();
            }
        }
        foreach (var dict in availableItems)
        {
            var scene = GD.Load<PackedScene>(FolderConstants.Scenes + "card/shop_item_card.tscn");
            if (scene == null) continue;
            var card = scene.Instantiate<Control>();
            GD.Print(dict);
            // 假设卡片有Init(dict)方法
            AddChild(card);
            if (card is ShopItemCard storeCard)
            {
                string itemName = dict.ContainsKey("name") ? dict["name"].ToString() : "Pea";
                string desc = dict.ContainsKey("desc") ? dict["desc"].ToString() : "解锁使用权";
                string title = dict.ContainsKey("title") ? dict["title"].ToString() : "解锁使用权";
                float itemX = dict.ContainsKey("x") ? dict["x"].AsSingle() : 155f;
                float itemY = dict.ContainsKey("y") ? dict["y"].AsSingle() : 75f;
                int Price = dict.ContainsKey("price") ? dict["price"].AsInt32() : 0;
                float itemViewScale = dict.ContainsKey("viewscale") ? dict["viewscale"].AsSingle() : 1f;
                storeCard.Init(itemName, Price, title, desc, itemX, itemY, itemViewScale);
            }
        }
    }
}
