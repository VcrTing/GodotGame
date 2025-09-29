using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;

using ZVB4.Interface;
using ZVB4.Tool;

namespace ZVB4.Tool
{
    public static class ObjTool
    {
        // 获取伤害值
        public static int GetObjDamage(EnumObjType objType, string name)
        {
            switch (objType)
            {
                case EnumObjType.Plans:
                    return DmageConstants.GetPlansDamage(name);
                case EnumObjType.Zombie:
                    return EnmyTypeConstans.GetZombieDamage(name);
                default:
                    break;
            }
            return 0;
        }
        public static int GetObjDamageExtra(EnumObjType objType, string name)
        {
            switch (objType)
            {
                case EnumObjType.Plans:
                    return DmageConstants.GetPlansDamageExtra(name);
                case EnumObjType.Zombie:
                    return 0;
                default:
                    break;
            }
            return 0;
        }

        // 获取额外血量
        public static int GetYouExtraHealth(EnumObjType objType, string name)
        {
            switch (objType)
            {
                case EnumObjType.Zombie:
                    // 获取health值
                    int extraHp = EnmyTypeConstans.GetExtraHp(name);
                    if (extraHp >= 0)
                    {
                        return extraHp;
                    }
                    break;
                case EnumObjType.Plans:
                    return 0;
                default:
                    break;
            }
            return (int)EnumHealth.One;
        }
        public static int GetYouHealth(EnumObjType objType, string name)
        {
            switch (objType)
            {
                case EnumObjType.Zombie:
                    // 获取health值
                    int hp = EnmyTypeConstans.GetHp(name);
                    if (hp >= 0)
                    {
                        return hp;
                    }
                    break;
                case EnumObjType.Plans:
                    return PlansConstants.GetPlansHealth(name);
                default:
                    break;
            }
            return (int)EnumHealth.One;
        }

        public static bool RunningBeHurt(Node2D node, EnumObjType objType, int damage, EnumHurts enumHurts)
        {
            bool died = false;
            // 执行东西受伤
            bool hasExtra = false;
            // 溢出伤害
            int damageYiChu = 0;
            var viewExtra = node.GetNodeOrNull<Node>(NameConstants.ViewExtra);
            if (viewExtra != null)
            {
                if (viewExtra is IHealth h)
                {
                    bool isDead = h.IsDie();
                    if (!isDead)
                    {
                        int yichu = h.CostHealth(objType, damage, enumHurts);
                        if (yichu > 0) damageYiChu = yichu;
                        hasExtra = true;
                    }
                }
            }
            // GD.Print($"{Name} 受伤，伤害 {damage}, damageYiChu: {damageYiChu}. ");
            // 没有 Extra
            if (!hasExtra || damageYiChu > 0)
            {
                damage += damageYiChu; // 加上溢出伤害
                var view = node.GetNodeOrNull<Node>(NameConstants.View);
                if (view is IHealth h)
                {
                    int yichu = h.CostHealth(objType, damage, enumHurts);
                    bool isDead = h.IsDie();
                    GD.Print($"{node.Name} 受伤，伤害 {isDead}. ");
                    if (isDead)
                    {
                        GD.Print($"{node.Name} 死亡. ");
                        died = true;
                    }
                }
            }
            return died;
        }
        
        public static bool RunningDie(Node2D node, EnumObjType objType, int damage, EnumHurts enumHurts)
        {
            try {
                // 立刻销毁Working
                var workingNode = node.GetNodeOrNull<Node>(NameConstants.Working);
                if (workingNode != null)    
                {
                    workingNode.QueueFree();
                }
                // 立刻销毁AttackArea
                    var attackArea = node.GetNodeOrNull<Node>(NameConstants.AttackArea);
                if (attackArea != null)
                {
                    attackArea.QueueFree();
                }
                // 立刻销毁BeHurtArea
                var beHurtArea = node.GetNodeOrNull<Node>(NameConstants.BeHurtArea);
                if (beHurtArea != null)
                {
                    beHurtArea.QueueFree();
                }
                // 立刻销毁BodyMove
                var bodyMove = node.GetNodeOrNull<Node>(NameConstants.BodyMove);
                if (bodyMove != null)
                {
                    bodyMove.QueueFree();
                }
            }
            catch (Exception ex)
            {
                GD.PrintErr($"ObjTool.RunningDie 出错: {ex.Message} ");
            }
            // 立刻结束工作
            IWorking working = node as IWorking;
            if (working != null)
            {
                working.SetWorkingMode(false);
            }
            // 0.5f 后，销毁自己
            return true;
        }
    }
}