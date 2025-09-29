using ZVB4.Interface;

namespace ZVB4.Conf
{
    public static class AnimationConstants
    {
        public const float BulletFadeDieDuration = 0.5f;

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
    }
}
