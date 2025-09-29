using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class BulletWall : Area2D
{
    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        // 提取IObj，判断GetObjName==BulletConstants.BulletPea
        var obj = area.GetParent() as IObj;
        if (obj != null && obj.GetObjName() == BulletConstants.BulletPeaName)
        {
            // 执行某个方法，例如Die()
            FroBullet(area, obj);
        }
    }

    void FroBullet(Area2D area, IObj obj)
    {
        IBulletBase bulletBase = area.GetParent<IBulletBase>();
        // 
        bulletBase?.FlipXDirection();
    }
}
