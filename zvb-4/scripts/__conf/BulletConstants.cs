
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class BulletConstants
    {
        public const string BulletPeaName = "BulletPea";
        public const string BulletLanMeiName = "BulletLanMei";
        public const string BulletYangTaoName = "BulletYangTao";
        public const string BulletXiguaBingName = "BulletXiguaBing";

        public const float LiveTimeTotal = 8f;

        public const int DamagePea = 500;
        public const int DamageGroupPea = 0;
        public const int DamageExtraPea = 0;
        //
        public const int DamageLanMei = 700;
        public const int DamageGroupLanMei = 200;
        public const int DamageExtraLanMei = 0;
        //
        public const int DamageYangTao = 500;
        public const int DamageGroupYangTao = 0;
        public const int DamageExtraYangTao = 0;
        //
        public const int DamageXiguaBing = 3000;
        public const int DamageGroupXiguaBing = 1000;
        public const int DamageExtraXiguaBing = 1000;

        public static readonly Dictionary<string, int> BulletDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamagePea },
            { BulletLanMeiName, DamageLanMei },
            { BulletYangTaoName, DamageYangTao },
            { BulletXiguaBingName, DamageXiguaBing },
        };
        public static readonly Dictionary<string, int> BulletGroupDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamageGroupPea },
            { BulletLanMeiName, DamageGroupLanMei },
            { BulletYangTaoName, DamageGroupYangTao },
            { BulletXiguaBingName, DamageGroupXiguaBing },
        };

        public static readonly Dictionary<string, int> BulletExtraDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamageExtraPea },
            { BulletLanMeiName, DamageExtraLanMei },
            { BulletYangTaoName, DamageExtraYangTao },
            { BulletXiguaBingName, DamageExtraXiguaBing },
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

        public const float SpeedPea = 800f;
        public const float SpeedLanMei = 600f;
        public const float SpeedYangTao = 1000f;
        public const float SpeedXiguaBing = 1120f;
        public static float GetSpeed(string n)
        {
            return n switch
            {
                BulletPeaName => SpeedPea,
                BulletLanMeiName => SpeedLanMei,
                BulletYangTaoName => SpeedYangTao,
                BulletXiguaBingName => SpeedXiguaBing,
                _ => 700f,
            };
        }
    }

}