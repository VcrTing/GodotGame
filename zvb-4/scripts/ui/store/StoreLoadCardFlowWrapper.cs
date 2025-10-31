using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class StoreLoadCardFlowWrapper : FlowContainer
{
    [Export]
    public string CardsJsonPath;
    List<Godot.Collections.Dictionary> availableItems = new List<Godot.Collections.Dictionary>();
    float __t = 0f;
    float __delay = 1f;
    public override void _Process(double delta)
    {
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
    }
    string GetJsonPath() => FolderConstants.Designs + "store/" + CardsJsonPath;
    //
    public void LoadAndFilterItems()
    {
        availableItems = CommonTool.LoadJsonToListDict(GetJsonPath());
        GenerateStoreCards();
    }
    // 批量生成卡片节点
    public void GenerateStoreCards()
    {
        if (availableItems.Count == 0) return;
        List<Control> controls = GodotTool.GenerateControlList(this, availableItems.Count, FolderConstants.Scenes + "card/in_game_store_item_card.tscn");
        for (int i= 0; i< availableItems.Count; i++)
        {
            Control c = controls[i];
            var dict = availableItems[i];
            AddChild(c);
            if (c is InGameStoreItemCard storeCard)
            {
                string itemName = dict.ContainsKey("name") ? dict["name"].ToString() : "Pea";
                float itemX = dict.ContainsKey("x") ? dict["x"].AsSingle() : 155f;
                float itemY = dict.ContainsKey("y") ? dict["y"].AsSingle() : 75f;
                int Price = dict.ContainsKey("price") ? dict["price"].AsInt32() : 0;
                float itemViewScale = dict.ContainsKey("viewscale") ? dict["viewscale"].AsSingle() : 1f;
                storeCard.Init(itemName, Price, itemX, itemY, itemViewScale);
            }
        }
    }
}
