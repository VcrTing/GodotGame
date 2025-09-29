using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class PlansBeHurtArea : Area2D, IHurtBase
{
    public bool TakeDamage(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (enumHurts == EnumHurts.Bait)
        {
            var p = GetParent();
            IBeHurt beHurt = p as IBeHurt;
            if (beHurt != null)
            {
                beHurt.BeHurt(objType, damage, enumHurts);
            }
            return true;
        }
        return false;
    }
}
