
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class BulletConstants
    {
        public const string BulletPeaName = "BulletPea";
        public const string BulletXiguaBingName = "BulletXiguaBing";

        public const float LiveTimeTotal = 8f;

        public const int DamagePea = 500;
        public const int DamageExtraPea = 0;
        public const int DamageXiguaBing = 3000;
        public const int DamageExtraXiguaBing = 1000;

        public static readonly Dictionary<string, int> BulletDamageDict = new Dictionary<string, int>
        {
            { BulletPeaName, DamagePea },
            { BulletXiguaBingName, DamageExtraPea },
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
        public static int GetDamageExtra(string n)
        {
            if (BulletExtraDamageDict.TryGetValue(n, out int dmg))
                return dmg;
            return 0;
        }

        // 子弹初始速度

        public const float SpeedPea = 700f;
        public const float SpeedXiguaBing = 400f;
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