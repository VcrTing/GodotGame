using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class BuffItemCard : Node2D, IWorking
{
    [Export]
    public EnumPlayerBuff PlayerBuff { get; set; } = EnumPlayerBuff.None;

    public void SetPalyerBuff(EnumPlayerBuff buff)
    {
        Init();
        PlayerBuff = buff;
        UpdateBuffView();
    }   

    Label DamageLabel;
    Label SpeedLabel;

    Node2D DamageNode;
    Node2D SpeedNode;

    void Init()
    {
        DamageNode = GetNodeOrNull<Node2D>("BuffDamage");
        SpeedNode = GetNodeOrNull<Node2D>("BuffSpeed");
        if (DamageNode == null || SpeedNode == null)
        {
            GD.PrintErr("BuffItemCard: Buff view nodes not found");
            return;
        }
        DamageLabel = DamageNode?.GetNodeOrNull<Label>("DamageLabel");
        SpeedLabel = SpeedNode?.GetNodeOrNull<Label>("SpeedLabel");
    }

    public override void _Ready()
    {
        Init();
    }

    ShootBuff sb = new ShootBuff();
    void ViewDamage(EnumPlayerBuff v)
    {
        DamageNode.Visible = true;
        SpeedNode.Visible = false;
        string vv = ((int)v).ToString();
        int __v = vv.Substring(1, vv.Length - 1) is string s && int.TryParse(s, out int result) ? result : 0;
        DamageLabel.Text = $"伤害+{__v}0%";
        sb.attackDamageRatio = 1f + (__v * 0.1f);
    }
    void ViewSpeed(EnumPlayerBuff v)
    {
        DamageNode.Visible = false;
        SpeedNode.Visible = true;
        string vv = ((int)v).ToString();
        int __v = vv.Substring(1, vv.Length - 1) is string s && int.TryParse(s, out int result) ? result : 0;
        SpeedLabel.Text = $"攻速+{__v}0%";
        sb.attackSpeedRatio = 1f + (__v * 0.1f);
    }
    void UpdateBuffView()
    {
        if (PlayerBuff == EnumPlayerBuff.None)
        {
            DamageNode.Visible = false;
            SpeedNode.Visible = false;
            return;
        }
        string vv = PlayerBuff.ToString();
        if (vv.StartsWith("Speed"))
        {
            ViewSpeed(PlayerBuff);
        }
        else if (vv.StartsWith("Damage"))
        {
            ViewDamage(PlayerBuff);
        }
    }

    void WorkingBuff()
    {
        var player = PlayerController.Instance;
        if (player == null)
            return;
        player.AddShootBuff(sb);
        QueueFree();
    }

    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        if (isWorking)
        {
            WorkingBuff();
        }
    }

    public bool IsWorking() => isWorking;

}
