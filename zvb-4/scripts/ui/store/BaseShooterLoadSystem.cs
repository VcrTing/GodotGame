using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class BaseShooterLoadSystem : FlowContainer
{
    [Export]
    public string CardsJsonPath = "choise/base_shooters.json";
    List<Godot.Collections.Dictionary> availableItems = new List<Godot.Collections.Dictionary>();
    float __t = 0f;
    float __delay = 1f;
    public static BaseShooterLoadSystem Instance { get; private set; }
    public override void _Ready() { Instance = this; LoadAndFilterItems(); }
    public override void _Process(double delta)
    {
        
    }
    string GetJsonPath() => FolderConstants.Designs + "/" + CardsJsonPath;
    //
    public async void LoadAndFilterItems()
    {
        var sys = SaveDataManager.Instance;
        if (sys == null)
        {
            GetTree().CreateTimer(0.2f).Timeout += () => LoadAndFilterItems();
            return;
        }
        availableItems = CommonTool.LoadJsonToListDict(GetJsonPath());
        GenerateStoreCards();
    }
    // 批量生成卡片节点
    public void GenerateStoreCards()
    {
        if (availableItems.Count == 0) return;
        List<Control> controls = GodotTool.GenerateControlList(this, availableItems.Count, FolderConstants.Scenes + "card/base_shooter_card.tscn");
        for (int i = 0; i < availableItems.Count; i++)
        {
            Control c = controls[i];
            var dict = availableItems[i];
            AddChild(c);
            //
            if (c is BaseShooterCard storeCard)
            {
                string itemName = dict.ContainsKey("name") ? dict["name"].ToString() : "Pea";
                float itemX = dict.ContainsKey("x") ? dict["x"].AsSingle() : 280f;
                float itemY = dict.ContainsKey("y") ? dict["y"].AsSingle() : 130f;
                int Price = dict.ContainsKey("price") ? dict["price"].AsInt32() : 0;
                float itemViewScale = dict.ContainsKey("viewscale") ? dict["viewscale"].AsSingle() : 1f;
                storeCard.Init(itemName, Price);
                storeCard.Init2(itemX, itemY, itemViewScale);
            }
        }
    }

    //
    public string shooterNow;
    public string GetBaseShooter()
    {
        string bs = SaveDataManager.Instance.GetLastBaseShooter();
        shooterNow = bs; return bs;
    }
    public void SetBaseShooter(string n)
    {
        if (n == null || n == "") return; shooterNow = n;
        SaveDataManager.Instance.TrySavePlayerShooterBaseLast(n);
    }

    public bool BuyThisBaseShooter(string name)
    {
        if (!PlansConstants.IsShooter(name)) return false;
        bool isbuyed = SaveDataManager.Instance.HasPlans(name);
        if (!isbuyed)
        {
            SaveDataManager.Instance.UnlockPlans(name);
        }
        SaveDataManager.Instance.UnLockShooterUnLimit(name);
        SwitchBaseShooter(name);
        return true;
    }
    public bool SwitchBaseShooter(string name)
    {
        if (!PlansConstants.IsShooter(name)) return false;
        SetBaseShooter(name);
        Godot.Collections.Array<Node> cards = GetChildren();
        if (cards.Count == 0) return false;
        for (int i = 0; i< cards.Count; i ++)
        {
            Node c = cards[i];
            BaseShooterCard cd = c as BaseShooterCard;
            if (cd == null) continue;
            cd.ChangeChoise(name);
            //
            ChoiseShooterDetailView.Instance.AsyncShooterView();
        }
        return true;
    }
}
