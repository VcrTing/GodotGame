using Godot;
using System;
using ZVB4.Conf;

public partial class MoneyCenterSystem : Node2D
{
    public static MoneyCenterSystem Instance { get; private set; }

    Node working;

    public override void _Ready()
    {
        Instance = this;
        working = GetNodeOrNull<Node2D>(NameConstants.Working);
    }

    public void AddMoney(int value)
    {
        if (SaveDataManager.Instance != null)
        {
            if (value < 0) { value = -value; }
            SaveDataManager.Instance.SetMoneyAndSave(value);
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
            // 使用SetDeferred避免物理状态错误
            AddChild(instance);
            // this.SetDeferred("add_child", instance);
        }
        catch
        {

        }
    }
}
