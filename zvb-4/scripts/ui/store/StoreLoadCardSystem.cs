using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ZVB4.Conf;

public partial class StoreLoadCardSystem : BoxContainer
{
    [Export]
    public string CardsJsonPath;
    public static StoreLoadCardSystem Instance { get; private set; }
    List<Godot.Collections.Dictionary> availableItems = new List<Godot.Collections.Dictionary>();
    public override void _Ready()
    {
        Instance = this;
        LoadAndFilterItems();
    }
    float __t = 0f;
    float __delay = 1f;
    public override void _Process(double delta)
    {

    }
    string GetJsonPath() => FolderConstants.Designs + "store/" + CardsJsonPath;
    public void LoadAndFilterItems()
    {
        availableItems = CommonTool.LoadJsonToListDict(GetJsonPath());
        GenerateStoreCards();
    }
    // 批量生成卡片节点
    public void GenerateStoreCards()
    {
        if (availableItems.Count == 0) return;
        List<Control> controls = GodotTool.GenerateControlList(this, availableItems.Count, FolderConstants.Scenes + "card/shop_item_card.tscn");
        for (int i = 0; i < availableItems.Count; i++)
        {
            Control c = controls[i];
            var dict = availableItems[i];
            AddChild(c);
            if (c is ShopItemCard storeCard)
            {
                int id = GetItemId(dict);
                string itemName = dict.ContainsKey("name") ? dict["name"].ToString() : "Pea";
                string desc = dict.ContainsKey("desc") ? dict["desc"].ToString() : "解锁使用权";
                string title = dict.ContainsKey("title") ? dict["title"].ToString() : "解锁使用权";
                float itemX = dict.ContainsKey("x") ? dict["x"].AsSingle() : 155f;
                float itemY = dict.ContainsKey("y") ? dict["y"].AsSingle() : 75f;
                int Price = dict.ContainsKey("price") ? dict["price"].AsInt32() : 0;
                int Buyed = dict.ContainsKey("buyed") ? dict["buyed"].AsInt32() : 0;
                float itemViewScale = dict.ContainsKey("viewscale") ? dict["viewscale"].AsSingle() : 1f;
                //
                storeCard.Init(itemName, Price, title, desc);
                storeCard.Init2(id, Buyed, itemX, itemY, itemViewScale);
            }
        }
    }
    int GetItemId(Godot.Collections.Dictionary dict) => dict.ContainsKey("id") ? dict["id"].AsInt32() : 0;
    System.Collections.Generic.Dictionary<string, object> toSystemDict(Godot.Collections.Dictionary dict)
    {
        System.Collections.Generic.Dictionary<string, object> src = new System.Collections.Generic.Dictionary<string, object>();
        //
        int id = GetItemId(dict);
        string itemName = dict.ContainsKey("name") ? dict["name"].ToString() : "Pea";
        string desc = dict.ContainsKey("desc") ? dict["desc"].ToString() : "解锁使用权";
        string title = dict.ContainsKey("title") ? dict["title"].ToString() : "解锁使用权";
        float itemX = dict.ContainsKey("x") ? dict["x"].AsSingle() : 100f;
        float itemY = dict.ContainsKey("y") ? dict["y"].AsSingle() : 100f;
        int Price = dict.ContainsKey("price") ? dict["price"].AsInt32() : 0;
        int Buyed = dict.ContainsKey("buyed") ? dict["buyed"].AsInt32() : 0;
        float itemViewScale = dict.ContainsKey("viewscale") ? dict["viewscale"].AsSingle() : 1f;
        //
        src["id"] = id;
        src["name"] = itemName;
        src["desc"] = desc;
        src["title"] = title;
        src["x"] = itemX;
        src["y"] = itemY;
        src["price"] = Price;
        src["buyed"] = Buyed;
        src["viewscale"] = itemViewScale;
        //
        return src;
    }
    // 
    public bool SetItemBuyed(int itemId, bool isBuyed)
    {
        List<System.Collections.Generic.Dictionary<string, object>> res = new List<System.Collections.Generic.Dictionary<string, object>>();
        string plansName = null;
        foreach (var dict in availableItems)
        {
            int id = GetItemId(dict);
            if (id == itemId) {
                dict["buyed"] = isBuyed ? 1 : 0;
                plansName = dict["name"].AsString();
            }
            System.Collections.Generic.Dictionary<string, object> one = toSystemDict(dict);
            res.Add(one);
        }
        bool src = CommonTool.WriteDataToJson(res, GetJsonPath());
        if (src && plansName != null)
        {
            GD.Print("顺便解锁植物 " + plansName);
            SaveDataManager.Instance.UnlockPlans(plansName);
        }
        return src;
    }
}
