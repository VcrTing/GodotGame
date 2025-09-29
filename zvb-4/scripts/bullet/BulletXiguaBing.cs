using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class BulletXiguaBing : Node2D, IObj, IBulletBase
{
    public Vector2 Direction { get; set; } = Vector2.Up; // 默认向上

    public override async void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
    }
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, maxScale, minScale);
        return true;
    }

    public bool Die()
    {
        throw new NotImplementedException();
    }

    public EnumObjType GetEnumObjType()
    {
        throw new NotImplementedException();
    }

    public string GetObjName()
    {
        throw new NotImplementedException();
    }

    public bool Init(string name = null)
    {
        throw new NotImplementedException();
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction;
    }
    public void FlipXDirection()
    {
        Direction = new Vector2(-Direction.X, Direction.Y);
    }
}
