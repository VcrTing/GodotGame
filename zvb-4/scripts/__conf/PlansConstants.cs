
namespace ZVB4.Conf
{
    public static class PlansConstants
    {
        public const string Miao = "Miao";
        public const string Pea = "Pea";
        public const string XiguaBing = "XiguaBing";

        public const string SunFlower = "SunFlower";

        public const string Cherry = "Cherry";

        // 植物名，卡片
        public static readonly System.Collections.Generic.Dictionary<string, string> PlanSceneDict = new System.Collections.Generic.Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlans + "plansshooter/pea.tscn" },
            { XiguaBing, FolderConstants.WavePlans + "plansshooter/xigua_bing.tscn" },
            { SunFlower, FolderConstants.WavePlans + "plansfuzhu/sun_flower.tscn" },
            { Cherry,  FolderConstants.WavePlans + "plansonce/cherry.tscn" }
        };

        // 植物名，shooter
        public static readonly System.Collections.Generic.Dictionary<string, string> ShooterDict = new System.Collections.Generic.Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlayer + "shooter.tscn" },
            { XiguaBing, FolderConstants.WavePlayer + "shooter.tscn" },
        };
        // 植物名，bullet
        public static readonly System.Collections.Generic.Dictionary<string, string> BulletDict = new System.Collections.Generic.Dictionary<string, string>
        {
            { Pea, FolderConstants.WaveBullet + "bullet_zero.tscn" },
            { XiguaBing, FolderConstants.WaveBullet + "bullet_xigua_bing.tscn" },
        };

        // 植物生长时长
        public static readonly System.Collections.Generic.Dictionary<string, float> PlanGrowTimeDict = new System.Collections.Generic.Dictionary<string, float>
        {
            { Pea, 3f },
            { XiguaBing, 5f },
            { SunFlower, 1f },
            { Cherry, 2f }
        };

        // 植物生命值
        public static readonly System.Collections.Generic.Dictionary<string, int> PlanHealthDict = new System.Collections.Generic.Dictionary<string, int>
        {
            { Pea, (int)EnumHealth.One },
            { XiguaBing, (int)EnumHealth.Four },
            { SunFlower, (int)EnumHealth.Two },
            { Cherry, (int)EnumHealth.Four }
        };
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

    }

}