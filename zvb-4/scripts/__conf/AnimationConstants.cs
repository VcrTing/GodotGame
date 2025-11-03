using ZVB4.Interface;

namespace ZVB4.Conf
{
    public static class AnimationConstants
    {
        public const string AniDefault = "default";
        public const string AniWorking = "working";

        public const string AniIdle = "idle";
        public const string AniWalk = "walk";
        public const string AniAttack = "attack";
        public const string AniWalk60 = "walk60";
        public const string AniWalk30 = "walk30";

        public const string AniDieForBoom = "dieforboom";
        public const string AniOutro = "outro";
        public const string AniIntro = "intro";
        public const float DurationDieForBoom = 0.5f;
        public const float DurationOutro = 0.5f;
        public const float DurationIntro = 0.5f;

        public const float BulletFadeDieDuration = 0.5f;
        public const float BulletXiGuaFadeDieDuration = 0.5f;

        // 获取 死亡动画时长
        public static float GetDieAniTime(IObj obj)
        {
            EnumObjType objType = obj.GetEnumObjType();
            string key = obj.GetObjName();
            switch (objType)
            {
                case EnumObjType.Plans:
                    return GetPlantDieAniTime(key);
                case EnumObjType.Zombie:
                    return GetZombieDieAniTime(key);
                default:
                    return 0.5f;
            }
        }

        static float GetPlantDieAniTime(string key)
        {
            switch (key)
            {
                default:
                    return 0.7f;
            }
        }
        static float GetZombieDieAniTime(string key)
        {
            switch (key)
            {
                default:
                    return 0.7f;
            }
        }

        // 获取死亡动画时长
        public static float GetViewDieAniTime(IObj obj)
        {
            EnumObjType objType = obj.GetEnumObjType();
            string key = obj.GetObjName();
            switch (objType)
            {
                default:
                    return 0.6f;
            }
        }

        //
        public static float MoveWalkRatio = 1f;
        public static float MoveWalkFastRatio = 1.6f;
        public static float MoveRunRatio = 2.5f;
        public static float MoveRunFastRatio = 4f;
        
        public static float MoveWalkToPlayRatio = 1.2f;
        public static float MoveRunToPlayRatio = 2.7f;

    }
}
