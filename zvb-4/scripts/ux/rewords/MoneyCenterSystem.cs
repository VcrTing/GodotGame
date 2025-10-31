using Godot;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using ZVB4.Conf;

public partial class MoneyCenterSystem : Control
{
    [Export]
    public bool IsAlwaysShow = false;
        
    public static MoneyCenterSystem Instance { get; private set; }

    private Label _countShowLabel;
    public int v = 0;
    public int prev = 0;

    public override void _Ready()
    {
        Instance = this;
        Visible = false;
        if (SaveDataManager.Instance == null)
        {
            GetTree().CreateTimer(0.1f).Timeout += () => _Ready();
            return;
        }
        v = SaveDataManager.Instance.GetMoney();
        prev = SaveDataManager.Instance.GetMoney();
        UpdateMoneyLabel(v);
        AsyncMoneyValue();
        ShowMe();
        __vt = 0.0001f;
    }
    bool show = false;
    float stayTime = 2f;
    public async void ShowMe()
    {
        if (show) return;
        show = true;
        Visible = true;
        if (IsAlwaysShow) return;
        await ToSignal(GetTree().CreateTimer(stayTime), "timeout");
        show = false;
        Visible = false;
    }
    public void AddMoney(int value)
    {
        if (SaveDataManager.Instance != null)
        {
            if (value < 0) { value = -value; }
            v += value;
            // GD.Print("vv" + value + " v =" + v);
            UpdateMoneyLabel(v);
            ShowMe();
        }
    }
    // 第5关 保底 950 金币
    public void AsyncMoneyValue()
    {
        __vt = 0.0001f;
        if (SaveDataManager.Instance != null)
        {
            SaveDataManager.Instance.AddMoneyAndSave(v - prev);
            UpdateMoneyLabel(v);
            prev = v;
            stayTime = 2f;
        }
    }

    public bool CostForBuyed(int price)
    {
        int v = SaveDataManager.Instance.GetMoney();
        if (v < price) return false;
        SaveDataManager.Instance.SetMoneyAndSave(v - price);
        UpdateMoneyLabel(v - price);
        return true;
    }

    float __vt = 0f;

    public override void _Process(double delta)
    {
        if (__vt > 0f)
        {
            __vt += (float)delta;
            if (__vt > 0.5f)
            {
                AsyncMoneyValue();
            }
        }
    }
    
    private void UpdateMoneyLabel(int v)
    {
        if (_countShowLabel == null)
        {
            _countShowLabel = GodotTool.FindLabelByName(this, NameConstants.CountShowLabel);
        }
        if (_countShowLabel != null)
        {
            _countShowLabel.Text = v.ToString();
        }
    }

    float w = GameContants.ScreenHalfW - 90;
    // 随机地点出生苗
    public void DumpMoneyRandomPosition(string name, bool playSound = true)
    {
        Vector2 pos = this.GlobalPosition;
        float x = pos.X;
        float y = pos.Y;
        //
        int v = (int)GD.RandRange(1, w);
        int v2 = (int)GD.RandRange(1, w);
        //
        x += v;
        y += v2;
        DumpMoney(new Vector2(x, y), name, playSound);
    }

    int countMoney = 0;
    public void DumpMoney(Vector2 position, string name, bool playSound)
    {
        try
        {
            var scene = GD.Load<PackedScene>(FolderConstants.WaveObj + SunMoneyConstants.MoneyS + ".tscn");
            var instance = scene.Instantiate<Node2D>();
            instance.Position = position;
            countMoney += 1;
            string n = SunMoneyConstants.MoneyS + countMoney;
            instance.Name = n;
            // 设置名称
            if (playSound)
            {
                // SoundFxController.Instance?.PlayFx("Ux/coll", "coll_sun", 4);
            }
            if (instance == null || instance.Name == "") return;
            GetTree().CurrentScene.AddChild(instance);
        }
        catch
        {

        }
    }

    public Vector2 GetLabelPosition()
    {
        return new Vector2(GameContants.ScreenHalfW, GameContants.ScreenHalfH);
    }
}
