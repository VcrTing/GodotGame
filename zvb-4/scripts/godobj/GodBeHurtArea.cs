using Godot;
using System;

using ZVB4.Interface;
using ZVB4.Conf;

public partial class GodBeHurtArea : Area2D, IHurtBase
{
    GodobjWrapper Wrapper => GetParent<GodobjWrapper>();


    public bool TakeDamage(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        // TODO: 这里可以实现具体的受击反馈或血量处理
        if (Wrapper.objType == EnumObjType.Zombie)
        {
            // GD.Print($"GodBeHurtArea 被 {bulletsType} 攻击，伤害：{damage}");
            var p = GetParent();
            IBeHurt beHurt = p as IBeHurt;
            if (beHurt != null)
            {
                // GD.Print($"GodBeHurtArea 被攻击，伤害：{damage} 方式: {enumHurts}");
                beHurt.BeHurt(objType, damage, enumHurts);
            }

            return true;
        }
        if (Wrapper.objType == EnumObjType.Stone)
        {
            return true;
        }
        return false;
    }
    
}
