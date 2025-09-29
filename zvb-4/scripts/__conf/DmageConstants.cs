using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class DmageConstants
    {

        // 植物伤害
        public const int  Pea = 0;
        public const int  ExtraPea = 0;
        public const int  XiguaBing = 0;
        public const int  ExtraXiguaBing = 0;

        public const int PlansCherry = 1000 * 10;
        public const int PlansCherryExtra = 10;

        // 植物伤害
        public static readonly Dictionary<string, int> PlantDamageDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea,  Pea },
            { PlansConstants.XiguaBing,  XiguaBing },
            { PlansConstants.Cherry, PlansCherry },
            //
            { PlansConstants.Miao, 0 },
            { PlansConstants.SunFlower, 0 },
        };
        // 植物额外伤害
        public static readonly Dictionary<string, int> PlantExtraDamageDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea,  ExtraPea },
            { PlansConstants.XiguaBing,  ExtraXiguaBing },
            { PlansConstants.Cherry, PlansCherryExtra },
            //
            { PlansConstants.Miao, 0 },
            { PlansConstants.SunFlower, 0 },
        };

        public static int GetPlansDamage(string plantName)
        {
            if (PlantDamageDict.TryGetValue(plantName, out int dmg))
                return dmg;
            return 0;
        }
        public static int GetPlansDamageExtra(string plantName)
        {
            if (PlantExtraDamageDict.TryGetValue(plantName, out int dmg))
                return dmg;
            return 0;
        }
    }
}
