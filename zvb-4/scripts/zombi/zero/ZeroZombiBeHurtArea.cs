using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ZeroZombiBeHurtArea : Area2D, IHurtBase
{
    // 
    IObj iobj;
    public override void _Ready()
    {
        iobj = GetParent<IObj>();
    }

    //
    public bool TakeDamage(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        // 我是僵尸
        if (iobj.GetEnumObjType() == EnumObjType.Zombie)
        {
            var p = GetParent();
            IBeHurt beHurt = p as IBeHurt;
            if (beHurt != null)
            {
                beHurt.BeHurt(objType, damage, enumHurts);
            }
            return true;
        }
        // 
        if (iobj.GetEnumObjType() == EnumObjType.Things)
        {
            return true;
        }
        return false;
    }
}
