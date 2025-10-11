using Godot;
using System;
using ZVB4.Interface;

public partial class GeZiWrapper : Node2D
{
	public static GeZiWrapper Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public IObj workingObj;

    // 设置当前工作对象
    public void SetWorkingObj(IObj obj)
    {
        workingObj = obj;
    }
}
