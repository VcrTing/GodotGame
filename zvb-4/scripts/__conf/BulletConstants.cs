
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class BulletConstants
    {
        public const string BulletPeaName = "BulletPea";
        public const string BulletXiguaBingName = "BulletXiguaBing";

        public const float LiveTimeTotal = 8f;

        public const int DamagePea = 500;
        public const int DamageGroupPea = 0;
        public const int DamageExtraPea = 0;

        //
        public const int DamageXiguaBing = 3000;
        public const int DamageGroupXiguaBing = 1000;
        public const int DamageExtraXiguaBing = 1000;

        public static readonly Dictionary<string, int> BulletDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamagePea },
            { BulletXiguaBingName, DamageXiguaBing },
        };
        public static readonly Dictionary<string, int> BulletGroupDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamageGroupPea },
            { BulletXiguaBingName, DamageGroupXiguaBing },
        };

        public static readonly Dictionary<string, int> BulletExtraDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamageExtraPea },
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
        public const float SpeedXiguaBing = 1120f;
        public static float GetSpeed(string n)
        {
            return n switch
            {
                BulletPeaName => SpeedPea,
                BulletXiguaBingName => SpeedXiguaBing,
                _ => 700f,
            };
        }
    }

}