using Godot;
using ZVB4.Conf;
using System;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansPea : Node2D, IObj
{
    AnimatedSprite2D view;

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View2In1(this, minScale, maxScale);
        return true;
    }
    public bool Die()
    {
        throw new NotImplementedException();
    }
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType()
    {
        return objType;
    }
    string objName = PlansConstants.Pea;
    public string GetObjName()
    {
        return objName;
    }

    public bool Init(string name = null)
    {
        return true;
    }


    public override void _Ready()
    {
        view = GodotTool.GetViewAndAutoPlay(this);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        AdjustView();
    }

}
