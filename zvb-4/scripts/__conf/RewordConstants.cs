
using System;
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

        static float MoneyBaseLv = 0.1f;
        public static float MoneyPrevSuccRewordSubLv = 0.1f;
        static float MiaoBaseLv = 2f; // 0.05f
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
            { EnmyTypeConstans.ZombiM, 0.01f },
            { EnmyTypeConstans.ZombiJi, 0.15f },
            { EnmyTypeConstans.ZombiMaozi, 0.03f },
            { EnmyTypeConstans.ZombiMice, 0.07f },
            { EnmyTypeConstans.ZombiBaozhi, 0.02f },
            { EnmyTypeConstans.ZombiGangMen, 0.02f },
            { EnmyTypeConstans.ZombiGlq, 0.1f },
            { EnmyTypeConstans.ZombiJuRen, 0.05f },
        };
        public static float GetEnmyDumpMiaoLv(string key)
        {
            if (ZombiPlansMiaoLvDict.TryGetValue(key, out var value))
                return value * MiaoBaseLv;
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


        static List<string> QieZiRewordList_1 = new List<string>()
        {
            PlansConstants.Pea, PlansConstants.XiHongShi, PlansConstants.JianGuo, PlansConstants.JianGuo,
            PlansConstants.Cherry, PlansConstants.LaJiao,
            PlansConstants.XianRenQiu, PlansConstants.IceFlower, PlansConstants.RewordFlower,
            PlansConstants.PeaCold, PlansConstants.PeaDouble, PlansConstants.YangTao, PlansConstants.ShiLiu, PlansConstants.LanMei,
            PlansConstants.Xigua, PlansConstants.PaoGu
        };
        static List<string> QieZiRewordList_2 = new List<string>()
        {
            PlansConstants.MoRiGu, PlansConstants.SunFlower, PlansConstants.SunGu, PlansConstants.LaJiao,
            PlansConstants.Yezi, PlansConstants.PeaGold, PlansConstants.XiguaBing,
        };
        static Random random = new Random();
        public static string GetQieZiRewordPlans()
        {
            List<string> tar = QieZiRewordList_1;
            int i = GD.RandRange(0, 100);
            if (i > 82) tar = QieZiRewordList_2;
            int randomIndex = random.Next(0, tar.Count);
            return tar[randomIndex];
        }
    }

}