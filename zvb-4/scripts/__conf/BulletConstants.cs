
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class BulletConstants
    {
        public const string BulletPeaName = "BulletPea";
        public const string BulletPeaColdName = "BulletPeaCold";
        public const string BulletPeaDoubleName = "BulletPeaDouble";
        public const string BulletShiLiuName = "BulletShiLiu";
        public const string BulletLanMeiName = "BulletLanMei";
        public const string BulletYangTaoName = "BulletYangTao";
        public const string BulletXiguaName = "BulletXigua";
        public const string BulletPaoGuName = "BulletPaoGu";
        //
        public const string BulletPeaGoldName = "BulletPeaGold";
        public const string BulletXiguaBingName = "BulletXiguaBing";
        public const string BulletYeziName = "BulletYezi";

        // 植物名，bullet
        public static readonly Dictionary<string, string> BulletDict = new Dictionary<string, string>
        {
            { PlansConstants.Pea, FolderConstants.WaveBullet + "bullet_zero.tscn" },
            { PlansConstants.PeaCold, FolderConstants.WaveBullet + "bullet_zero_cold.tscn" },
            { PlansConstants.PeaDouble, FolderConstants.WaveBullet + "bullet_zero_s.tscn" },
            //
            { PlansConstants.LanMei, FolderConstants.WaveBullet + "bullet_lan_mei.tscn" },
            { PlansConstants.YangTao, FolderConstants.WaveBullet + "bullet_yang_tao.tscn" },
            { PlansConstants.ShiLiu, FolderConstants.WaveBullet + "bullet_shi_liu.tscn" },
            { PlansConstants.Xigua, FolderConstants.WaveBullet + "bullet_xigua.tscn" },
            { PlansConstants.PaoGu, FolderConstants.WaveBullet + "bullet_pao_gu.tscn" },
            //
            { PlansConstants.Yezi, FolderConstants.WaveBullet + "diancang/bullet_yezi.tscn" },
            { PlansConstants.PeaGold, FolderConstants.WaveBullet + "diancang/bullet_pea_gold.tscn" },
            { PlansConstants.XiguaBing, FolderConstants.WaveBullet + "diancang/bullet_xigua_bing.tscn" },
        };
        

        public const float LiveTimeTotal = 8f;
        public const int Base = 10;
        public const int DamageBasic = Base * 50;
        public const int DamageOne = Base * 10;
        public const int Zero = 0;

        public static readonly Dictionary<string, int> BulletDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamageBasic },
            { BulletPeaColdName, Base * 60 },
            { BulletPeaDoubleName, DamageBasic },
            { BulletShiLiuName, Base * 40 },
            { BulletXiguaName, Base * 200 },

            { BulletPaoGuName, Base * 120 }, 
            //
            { BulletLanMeiName, Base * 70 },
            { BulletYangTaoName, Base * 50 },
            { BulletXiguaBingName, Base * 200 },
            { BulletPeaGoldName, Base * 400 },
            { BulletYeziName, Base * 600 },
        };
        public static readonly Dictionary<string, int> BulletGroupDamageDict = new Dictionary<string, int>
        {
            { BulletLanMeiName, Base * 20 },
            { BulletXiguaBingName, Base * 200 },
        };

        public static readonly Dictionary<string, int> BulletExtraDamageDict = new Dictionary<string, int>
        {
            { BulletXiguaName, Base * 100 },
            { BulletPeaDoubleName, DamageBasic },
            { BulletXiguaBingName, Base * 100 },
            { BulletYeziName, Base * 150 },
        };


        public static int GetDamage(string n)
        {
            if (BulletDamageDict.TryGetValue(n, out int dmg))
                return dmg;
            return 0;
        }
        public static int GetGroupDamage(string n)
        {
            if (BulletGroupDamageDict.TryGetValue(n, out int dmg))
                return dmg;
            return 0;
        }
        public static int GetDamageExtra(string n)
        {
            if (BulletExtraDamageDict.TryGetValue(n, out int dmg))
                return dmg;
            return 0;
        }

        // 子弹初始速度
        public const float SpeedBasic = 800f;

        public static float GetSpeed(string n)
        {
            return n switch
            {
                BulletPaoGuName => 300f,
                BulletPeaName => 800f,
                BulletPeaColdName => 700f,
                BulletPeaDoubleName => 920f,

                BulletShiLiuName => 1700f,
                BulletLanMeiName => 600f,
                BulletYangTaoName => 1000f,
                BulletXiguaName => 1480f,
                //
                BulletPeaGoldName => 2500f,
                BulletXiguaBingName => 1120f,
                BulletYeziName => 2000f,
                _ => 700f,
            };
        }
        
        // 根据key获取 Bullet 场景路径
        public static string GetBullet(string key)
        {
            if (BulletDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        // 其他植物
        public const int PlansDamageBase = DamageBasic;
        public const int DamageCherry = PlansDamageBase * 20;
        public const int DamageLaJiao = PlansDamageBase * 30;
        public const int DamageMoRiGu = PlansDamageBase * 50;
        public static int GetPlansDamage(string plantName)
        {
            if (plantName == PlansConstants.Cherry) return DamageCherry;
            else if (plantName == PlansConstants.LaJiao) return DamageLaJiao;
            else if (plantName == PlansConstants.MoRiGu) return DamageMoRiGu;
            else if (plantName == PlansConstants.XianRenQiu) return PlansDamageBase / 2;
            else if (plantName == PlansConstants.XiHongShi) return PlansDamageBase * 12;
            return 0;
        }
        public static int GetPlansDamageExtra(string plantName)
        {
            // if (plantName == PlansConstants.Cherry) return 10;
            // if (plantName == PlansConstants.LaJiao) return 16;
            return 0;
        }

        public static float GetPlansAttackSpeed(string plantName)
        {
            if (plantName == PlansConstants.XianRenQiu) return 1.4f;
            return 1f;
        }
    }

}