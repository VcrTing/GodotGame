using Godot;
using System;

public partial class Systems : Node2D
{
    public static Systems Instance { get; private set; }
    private int money = 0;
    int moneyDefault = 0;
    private Label moneyLabel;

    public override void _Ready()
    {
        Instance = this;
        Init();
    }

    void Init()
    {
        DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
        moneyLabel = GetNodeOrNull<Label>("Cvs/MoneyLabel");
        GD.Print("moneyLabel: " + moneyLabel);
        //
        LoadMoney();
        if (moneyLabel != null)
        {
            moneyLabel.Text = "金币：" + money;
        }
    }

    String __CFG_MONEY_FILE = "user://player_save.cfg";

    void LoadMoney()
    {
        // CCC: 读取本地储存的金币数
        var config = new Godot.ConfigFile();
        var err = config.Load(__CFG_MONEY_FILE);
        if (err == Error.Ok)
        {
            money = (int)config.GetValue("player", "gold", 0);
        }
        else
        {
            money = 0;
        }
        GD.Print("Loaded money: " + money);
        moneyDefault = money;
    }


    public void AddMoney(int value)
    {
        money = money + value;
        if (moneyLabel != null)
        {
            moneyLabel.Text = "金币：" + money;
        }
        // CCC: 存入本地储存
        var config = new Godot.ConfigFile();
        config.SetValue("player", "gold", money);
        config.Save(__CFG_MONEY_FILE);
    }

    public override void _Process(double delta)
    {
    }
}
