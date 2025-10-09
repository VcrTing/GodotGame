using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class PlansJianGuoBeHurtArea : Area2D, IBeHurt
{
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (objType == EnumObjType.Plans)
        {
            return false;
        }
        var parent = GetParent() as IBeHurt;
        if (parent != null)
        {
            return parent.BeHurt(objType, damage, enumHurts);
        }
        return false;
    }

    public Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        throw new NotImplementedException();
    }
}
