
using System;
using System.Linq;
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class PlansConstants
    {
        public const string Miao = "Miao";

        // 植物名
        public const string Pea = "Pea";
        public const string XiguaBing = "XiguaBing";

        public const string SunFlower = "SunFlower";

        public const string Cherry = "Cherry";

        // 植物名，卡片
        public static readonly  Dictionary<string, string> PlanSceneDict = new  Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlans + "plansshooter/pea.tscn" },
            { XiguaBing, FolderConstants.WavePlans + "plansshooter/xigua_bing.tscn" },
            { SunFlower, FolderConstants.WavePlans + "plansfuzhu/sun_flower.tscn" },
            { Cherry,  FolderConstants.WavePlans + "plansonce/cherry.tscn" }
        };

        // 植物名，shooter
        public static readonly  Dictionary<string, string> ShooterDict = new  Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlayer + "shooter.tscn" },
            { XiguaBing, FolderConstants.WavePlayer + "shooter.tscn" },
        };

        // 植物名，bullet
        public static readonly  Dictionary<string, string> BulletDict = new  Dictionary<string, string>
        {
            { Pea, FolderConstants.WaveBullet + "bullet_zero.tscn" },
            { XiguaBing, FolderConstants.WaveBullet + "bullet_xigua_bing.tscn" },
        };

        // 植物生长时长
        public static readonly  Dictionary<string, float> PlanGrowTimeDict = new  Dictionary<string, float>
        {
            { Pea, 2f },
            { XiguaBing, 5f },
            { SunFlower, 1f },
            { Cherry, 3f }
        };

        // 植物生命值
        public static readonly  Dictionary<string, int> PlanHealthDict = new  Dictionary<string, int>
        {
            { Pea, (int)EnumHealth.One },
            { XiguaBing, (int)EnumHealth.Four },
            { SunFlower, (int)EnumHealth.Two },
            { Cherry, (int)EnumHealth.Four }
        };

        // 不会占用格子的植物
        public static bool IsWillZhanYongGeZi(string planName)
        {
            if (planName == SunFlower)
            {
                return true;
            }
            return false;
        }
        
        // 射手限制
        public static readonly  Dictionary<string, int> ShooterAttackLimitDict = new  Dictionary<string, int>
        {
            { Pea, 0 },
            { XiguaBing, 8 },
        };
        public static int GetShooterAttackLimit(string key) {
            if (ShooterAttackLimitDict.TryGetValue(key, out var value))
                return value;
            return 0;
        }


        // 获取植物攻击速度
        public static float GetPlansAttackSpeedStart(string key)
        {
            if (key == Pea) return 0.4f;
            if (key == XiguaBing) return 1f;
            return 0f;
        }
        public static float GetPlansAttackSpeedSnap(string key)
        {
            if (key == Pea) return 0.07f;
            if (key == XiguaBing) return 0.1f;
            return 0f;
        }
        public static float GetPlansAttackSpeedEnd(string key)
        {
            if (key == Pea) return 0.15f;
            if (key == XiguaBing) return 0.2f;
            return 0f;
        }
        public static float GetPlansAttackSpeedSnapSnap(string key)
        {
            if (key == Pea) return 0.01f;
            if (key == XiguaBing) return 0.01f;
            return 0f;
        }

        // 根据key获取植物生命值
        public static int GetPlansHealth(string key)
        {
            if (PlanHealthDict.TryGetValue(key, out var value))
                return value;
            return 0;
        }
        // 根据key获取场景路径
        public static string GetPlanScene(string key)
        {
            if (PlanSceneDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        // 根据key获取Shooter场景路径
        public static string GetBullet(string key)
        {
            if (BulletDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        // 根据key获取Shooter场景路径
        public static string GetShooterScene(string key)
        {
            if (ShooterDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        // 根据key获取生长时长
        public static float GetPlanGrowTime(string key)
        {
            if (PlanGrowTimeDict.TryGetValue(key, out var value))
                return value;
            return 0f;
        }

        //
        
        public static string GetRandomPlansName()
        {
            var keys = PlanSceneDict.Keys.ToList();
            var rand = new Random();
            string key = keys[rand.Next(keys.Count)];
            return key;
        }

    }

}