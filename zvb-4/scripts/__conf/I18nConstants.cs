
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class I18nConstants
    {
        public static readonly Dictionary<string, string> PlanNameDict = new Dictionary<string, string>
        {
            { PlansConstants.Pea, "豌豆射手" },
            { PlansConstants.PeaCold, "寒冰豌豆" },
            { PlansConstants.PeaDouble, "双头豌豆" },

            { PlansConstants.LanMei, "蓝莓" },
            { PlansConstants.YangTao, "杨桃" },
            { PlansConstants.ShiLiu, "石榴" },
            { PlansConstants.Xigua, "西瓜" },
            
            { PlansConstants.RewordFlower, "金钱花" },
            { PlansConstants.SunFlower, "阳光花" },
            { PlansConstants.SunGu, "阳光菇" },
            { PlansConstants.XianRenQiu, "仙人球" },
            { PlansConstants.XiHongShi, "西红柿" },

            { PlansConstants.Cherry, "樱桃丸子" },
            { PlansConstants.JianGuo, "坚果墙" },
            { PlansConstants.IceFlower, "冰冻花" },
            { PlansConstants.LaJiao, "十字辣椒" },

            { PlansConstants.Yezi, "椰子樽" },
            { PlansConstants.XiguaBing, "冰西瓜" },
            { PlansConstants.PeaGold, "海洋金枪" },
        };
        
        public static string GetPlanChineseName(string key)
        {
            if (PlanNameDict.TryGetValue(key, out var value))
                return value;
            return key;
        }
    }
}