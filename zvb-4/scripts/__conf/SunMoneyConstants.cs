
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Godot;

namespace ZVB4.Conf
{
    public static class SunMoneyConstants
    {
        public const string Sun = "Sun";
        public const int SunNormal = 50;

        public const int SunLarge = 150;

        public const int FlowerGetSun = 50;
        public const int SkyGetSun = 50;

        public const int AttackCostPea = 10;

        public const int AttackCostXigua = 20;

        // 攻击消耗字典
        public static readonly Dictionary<string, int> AttackCostDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 10 },
            { PlansConstants.XiguaBing, 50 }
        };
        public static readonly Dictionary<string, int> DieSunDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 100 },
            { PlansConstants.XiguaBing, 300 }
        };

        public static int GetPlansSunCost(string plansName)
        {
            return AttackCostDict[plansName];
        }
        public static int GetDieSunCost(string plansName)
        {
            return DieSunDict[plansName];
        }
        public static int GetSunNumNormal(string plansName)
        {
            int n = DieSunDict[plansName];
            int v = n / SunNormal;
            if (v < 1) v = 1;
            return v;
        }
    }

}