using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ZeroZombiEyeArea : Area2D
{
	private bool _isClosed = false;

	public override void _Ready()
	{
		AreaEntered += OnAreaEntered;
	}

    private void OnAreaEntered(Area2D area)
    {
        if (_isClosed) return;
        var parent = area.GetParent();
        if (parent is IObj obj)
        {
            if (obj.GetEnumObjType() == EnumObjType.Plans)
            {
                _isClosed = true;

                // 切换状态和动画
            }
        }
    }
    

}
