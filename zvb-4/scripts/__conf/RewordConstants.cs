
using System.Collections.Generic;
using Godot;

namespace ZVB4.Conf
{
    public static class RewordConstants
    {
        public const string Sun = "Sun";
        public const string Money = "Money";
        public const string Plans = "Plans";
        public const string Buff = "Buff";

        // 
        public static readonly Dictionary<string, string> RewordSceneDict = new Dictionary<string, string>
        {
            { Sun, FolderConstants.WaveObj + "sun.tscn" },
        };

        // 根据key获取场景路径
        public static string GetRewordGroupScene(string key)
        {
            return FolderConstants.WaveObj + "reword_group.tscn";
        }
        public static string GetRewordScene(string key)
        {
            if (RewordSceneDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        static float MoneyBaseLv = 0.3f;
        public static float MoneyPrevSuccRewordSubLv = 0.1f;
        static float MiaoBaseLv = 0.05f;
        public static float MiaoPrevSuccRewordSubLv = 0.05f;
        public static readonly Dictionary<string, float> ZombiMoneyLvDict = new Dictionary<string, float>
        {
            { EnmyTypeConstans.ZombiS, 0.1f },
            { EnmyTypeConstans.ZombiMuTong, 0.1f },
            { EnmyTypeConstans.ZombiTieTong, 0.12f },
        };
        // 各种僵尸掉奖励概率
        public static float GetEnmyDumpMoneyLv(string key)
        {
            if (ZombiMoneyLvDict.TryGetValue(key, out var value))
                return value + MoneyBaseLv;
            return 0;
        }

        // 植物掉落奖励概率
        public static readonly Dictionary<string, float> ZombiPlansMiaoLvDict = new Dictionary<string, float>
        {
            { EnmyTypeConstans.ZombiS, 0.05f },
            { EnmyTypeConstans.ZombiMuTong, 0.05f },
            { EnmyTypeConstans.ZombiTieTong, 0.07f },
        };
        public static float GetEnmyDumpMiaoLv(string key)
        {
            if (ZombiPlansMiaoLvDict.TryGetValue(key, out var value))
                return value + MiaoBaseLv;
            return 0;
        }

        // 计算概率
        public static bool ComputedYesNo(float initLv, bool prevIsSuccReword, float succRewordSubLv)
        {
            float v = initLv;
            if (prevIsSuccReword)
            {
                v -= succRewordSubLv;
                v = v < 0f ? 0f : v;
            }
            if (GD.Randf() < v)
            {
                return true;
            }
            return false;
        }
    }

}