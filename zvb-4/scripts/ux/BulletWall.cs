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
        var obj = area.GetParent() as IObj;
        if (obj != null)
        {
            IBulletBase bulletBase = obj as IBulletBase;
            if (bulletBase != null)
            {
                FroBullet(area, bulletBase);
            }
        }
    }

    void FroBullet(Area2D area, IBulletBase ib)
    {
        // 
        ib?.FlipXDirection();
    }
}
