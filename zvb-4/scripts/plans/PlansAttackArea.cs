using Godot;
using System;

using ZVB4.Interface;
using ZVB4.Conf;

public partial class PlansAttackArea : Area2D
{
    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        LoadFather();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area == null) return;
        var parent = area.GetParent();
        if (parent is IObj obj)
        {
            var type = obj.GetEnumObjType();
            if (type == EnumObjType.Zombie || type == EnumObjType.Stone)
            {
                Attack(area, obj);
            }
        }
    }

    IObj myObj = null;
    IAttack myAttack = null;
    void LoadFather()
    {
        if (myObj != null || myAttack != null) return;
        myObj = this.GetParent() as IObj;
        myAttack = this.GetParent() as IAttack;
    }
    private void Attack(Area2D area, IObj areaObj)
    {
        LoadFather();
        int damage = myAttack?.GetDamage() ?? 0;
        // TODO: 实际攻击逻辑
        IHurtBase hurtBase = area as IHurtBase;
        if (hurtBase != null)
        {
            hurtBase.TakeDamage(myObj.GetEnumObjType(), damage, EnumHurts.Boom);
        }

    }
}
