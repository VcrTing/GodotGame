
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Godot;

namespace ZVB4.Conf
{
    public static class SunMoneyConstants
    {
        public const string Sun = "Sun";
        public const string MoneyG = "MoneyG";
        public const string Money = "Money";
        public const string MoneyS = "MoneyS";
        public const int SunBase = 10;
        public const int SunSmall = SunBase * 3;
        public const int SunNormal = SunBase * 5;
        public const int SunNormal2 = SunBase * 7;
        public const int SunLarge = SunBase * 10;

        // 攻击消耗字典
        public static readonly Dictionary<string, int> AttackCostDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 10 },
            { PlansConstants.PeaCold, 15 },
            { PlansConstants.PeaDouble, 20 },
            { PlansConstants.LanMei, 20 },
            { PlansConstants.YangTao, 30 },
            { PlansConstants.ShiLiu, 25 },
            { PlansConstants.Xigua, 35 },
            { PlansConstants.PaoGu, 15 },
            { PlansConstants.XiguaBing, 50 },
            { PlansConstants.PeaGold, 50 },
            { PlansConstants.Yezi, 100 }
        };
        public static readonly Dictionary<string, int> DieSunDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 50 },
            { PlansConstants.PeaCold, 65 },
            { PlansConstants.PeaDouble, 75 },
            { PlansConstants.LanMei, 75 },
            { PlansConstants.YangTao, 100 },
            { PlansConstants.PaoGu, 75 },
            { PlansConstants.ShiLiu, 80 },
            { PlansConstants.Xigua, 100 },
            { PlansConstants.XiguaBing, 200 },
            { PlansConstants.PeaGold, 200 },
            { PlansConstants.Yezi, 300 }    
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

        public static readonly Dictionary<string, string> PlansRewordType = new Dictionary<string, string>
        {
            { PlansConstants.SunFlower, Sun },
            { PlansConstants.RewordFlower, MoneyS },
            { PlansConstants.SunGu, Sun },
        };
        public static string GetPlansRewordType(string plansName) => PlansRewordType[plansName];
        //
        public static readonly Dictionary<string, int> SunPlansFirstGenTime = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, 2 },
            { PlansConstants.RewordFlower, 3 },
            { PlansConstants.SunGu, 3 },
        };
        public static int GetSunPlansFirstGenTime(string plansName) => SunPlansFirstGenTime[plansName];
        public static readonly Dictionary<string, int> SunPlansEveryGenTime = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, 8 },
            { PlansConstants.RewordFlower, 9 },
            { PlansConstants.SunGu, 9 },
        };
        public static int GetSunPlansEveryGenTime(string plansName) => SunPlansEveryGenTime[plansName];
        public static readonly Dictionary<string, int> SunPlansGrowTime = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, 1000 },
            { PlansConstants.RewordFlower, 999 },
            { PlansConstants.SunGu, 20 },
        };
        public static int GetSunPlansGrowTime(string plansName) => SunPlansGrowTime[plansName];

        //
        public static readonly Dictionary<string, int> SunPlansSunLevel1 = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, SunNormal },
            { PlansConstants.SunGu, SunSmall },
            { PlansConstants.RewordFlower, SunNormal },
        };
        public static int GetSunPlansSunLevel1(string plansName) => SunPlansSunLevel1[plansName];
        public static readonly Dictionary<string, int> SunPlansSunLevel2 = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, SunNormal },
            { PlansConstants.SunGu, SunNormal2 },
            { PlansConstants.RewordFlower, SunNormal },
        };
        public static int GetSunPlansSunLevel2(string plansName) => SunPlansSunLevel2[plansName];

        //
        public static readonly Dictionary<string, int> SunPlansSunCount1 = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, 1 },
            { PlansConstants.RewordFlower, 3 },
            { PlansConstants.SunGu, 1 },
        };
        public static int GetSunPlansSunCount1(string plansName) => SunPlansSunCount1[plansName];
        public static readonly Dictionary<string, int> SunPlansSunCount2 = new Dictionary<string, int>
        {
            { PlansConstants.SunFlower, 1 },
            { PlansConstants.RewordFlower, 4 },
            { PlansConstants.SunGu, 1 },
        };
        public static int GetSunPlansSunCount2(string plansName) => SunPlansSunCount2[plansName];
    }

}