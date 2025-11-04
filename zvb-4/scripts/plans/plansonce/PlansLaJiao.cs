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
    public string objName = PlansConstants.LaJiao;
    public string GetObjName() => objName;
    [Export]
    public int damage = 0;
    public int GetDamage() => damage;
    public int GetDamageExtra() => 0;

    public bool IsWorkingMode;
    public void SetWorkingMode(bool working)
    {
        ViewTool.View3In1(this, minScale, maxScale);
        if (working) { StartBoom(); }
        IsWorkingMode = working;
    }
    public void StartBoom() {  __t = 0.00001f; }
    bool isBoom = false;
    public void Boom()
    {
        if (isBoom) return; isBoom = true;
        // 播放音效
        SoundPlayerController.Instance.EnqueueSound("Plans/" + objName, objName, 4);
        //
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
        if (viewNode != null) viewNode.Visible = false;
    }
    public bool IsWorking() => IsWorkingMode;
    // 初始化
    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        ViewTool.View3In1(this, minScale, maxScale);
        damage = ObjTool.GetObjDamage(EnumObjType.Plans, objName);
        Init();
    }
    float __t = 0f;
    public override void _Process(double delta)
    {
        if (__t > 0f)
        {
            __t += (float)delta;
            if (__t > 1f)
            {
                Boom();
                if (__t > 1.7f) { Die(); }
            }
        }
    }
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool CanAttack() => IsWorkingMode;
    public bool Init(string name = null) => true;
    public bool Die()
    {
        QueueFree();
        return true;
    }
}
