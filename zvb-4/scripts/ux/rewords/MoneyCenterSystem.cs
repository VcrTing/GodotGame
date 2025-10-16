using Godot;
using System;
using System.Diagnostics;
using ZVB4.Conf;

public partial class MoneyCenterSystem : Control
{
    public static MoneyCenterSystem Instance { get; private set; }

    private Label _countShowLabel;
    public int v = 0;
    public int prev = 0;
    // Node working;

    public override void _Ready()
    {
        Instance = this;
        Visible = false;

        int a = SaveDataManager.Instance.GetMoney();
        v = a;
        prev = a;
        UpdateMoneyLabel(v);
        AsyncMoneyValue();
        ShowMe();
        SaveDataManager.Instance.SetMoneyAndSave(0);
    }
    bool show = false;
    float stayTime = 2f;
    public async void ShowMe()
    {
        if (show) return;
        show = true;
        Visible = true;
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
            UpdateMoneyLabel(v);
            ShowMe();
        }
    }
    void AsyncMoneyValue()
    {
        __vt = 0.0001f;
        if (SaveDataManager.Instance != null)
        {
            SaveDataManager.Instance.AddMoneyAndSave(prev - v);
            prev = v;
            UpdateMoneyLabel(v);
            stayTime = 2f;
        }
    }

    float __vt = 0f;

    public override void _Process(double delta)
    {
        if (__vt > 0f)
        {
            __vt -= (float)delta;
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
            var scene = GD.Load<PackedScene>(FolderConstants.WaveObj + "Money.tscn");
            var instance = scene.Instantiate<Node2D>();
            instance.Position = position;
            countMoney += 1;
            string n = "Money" + countMoney;
            instance.Name = n;
            // 设置名称
            if (playSound)
            {
                // SoundFxController.Instance?.PlayFx("Ux/coll", "coll_sun", 4);
            }
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
