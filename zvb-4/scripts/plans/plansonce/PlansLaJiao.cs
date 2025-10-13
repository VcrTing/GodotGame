using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansLaJiao : Node2D, IWorking, IObj, IAttack
{
    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.Cherry;
    public string GetObjName() => objName;
    [Export]
    public int damage = 0;
    public int GetDamage() => damage;
    [Export]
    public int damageExtra = 0;
    public int GetDamageExtra() => damageExtra;

    public bool IsWorkingMode;
    public void SetWorkingMode(bool working)
    {
        if (working) { Boom(); }
        IsWorkingMode = working;
    }
    public async void Boom()
    {
        // 1秒后爆炸
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        // 播放音效
        try
        {
            SoundPlayerController.Instance.EnqueueSound("Plans/" + objName, objName, 4);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error playing sound: {ex.Message}");
        }

        // CCC: 生成爆炸特效并隐藏自身View
        string boomScenePath = FolderConstants.WavePlans + "plansonce/inner/la_jiao_boom.tscn";
        var boomScene = GD.Load<PackedScene>(boomScenePath);
        if (boomScene != null)
        {
            var boom = boomScene.Instantiate<Node2D>();
            boom.Position = Position;
            this.AddChild(boom);
        }
        // 隐藏自身View子节点
        var viewNode = GetNodeOrNull<Node2D>(NameConstants.View);
        if (viewNode != null)
            viewNode.Visible = false;

        // 0.1秒后销毁
        await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
        Die();
    }
    public bool IsWorking() => IsWorkingMode;
    // 初始化
    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        AdjustView();
        Init();
    }
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        return true;
    }
    public bool CanAttack() => IsWorkingMode;
    public bool Init(string name = null)
    {
        damage = ObjTool.GetObjDamage(EnumObjType.Plans, objName);
        damageExtra = ObjTool.GetObjDamageExtra(EnumObjType.Plans, objName);
        return true;
    }
    public bool Die()
    {
        QueueFree();
        return true;
    }
}
