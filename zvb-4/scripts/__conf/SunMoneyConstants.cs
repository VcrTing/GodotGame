
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Godot;

namespace ZVB4.Conf
{
    public static class SunMoneyConstants
    {
        public const string Sun = "Sun";
        public const string MoneyG = "MoneyG";
        public const string MoneyS = "MoneyS";
        public const int SunNormal = 50;
        public const int SunLarge = 150;
        public const int FlowerGetSun = 50;
        public const int SkyGetSun = 50;

        // 攻击消耗字典
        public static readonly Dictionary<string, int> AttackCostDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 10 },
            { PlansConstants.PeaDouble, 20 },
            { PlansConstants.LanMei, 20 },
            { PlansConstants.YangTao, 30 },
            { PlansConstants.XiguaBing, 50 },
            { PlansConstants.PeaGold, 50 }
        };
        public static readonly Dictionary<string, int> DieSunDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 50 },
            { PlansConstants.PeaDouble, 75 },
            { PlansConstants.LanMei, 50 },
            { PlansConstants.YangTao, 100 },
            { PlansConstants.XiguaBing, 200 },
            { PlansConstants.PeaGold, 200 }
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
        //
        public static int GetMoneyValue(string n)
        {
            if (n == MoneyG) return 50;
            else if (n == MoneyS) return 25;
            return 25;
        }
        public static string GetRnadomMoneyName()
        {
            int i = GD.RandRange(1, 10);
            if (i > 7)
            {
                return MoneyG;
            }
            return MoneyS;
        }
        static float DefGravity = 1;
        public static float GetMoneyGravity()
        {
            int i = GD.RandRange(70, 200);
            return DefGravity * i / 100;
        }

    }

}