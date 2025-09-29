namespace ZVB4.Conf
{
    public static class GameContants
    {
        public const float TileW = 90f;
        public const float ZombieTileW = 70f;

        // 草地格子数量
        public const int GrassNum = 10;
        public const int GrassNumHalf = 5;

        // 花盆格子数量
        public const int FlowerPengNum = 8;
        public const int FlowerPengNumHalf = 4;

        // 屏幕一半
        public const float ScreenHalfW = 1080 / 2f;
        public const float ScreenHalfWX = 1080 / 1.8f;
        public const float ScreenHalfH = 1920 / 2f;

        // X 旋转角
        public const float RotateXAngleS = 70f;

        // 默认朝向 左
        public const int LookWhereDef = -1;

        // 物体内地图里的最小缩放
        public const float MinScale = 0.3f;
        // 物体内地图里的最大缩放
        public const float MaxScale = 1.0f;


        // 地平线位置
        public const float HorizonY = 500f;
        public const float HorizonBulletY = 500f;

        // Ui
        public const float UiLazyTouchTime = 0.2f;
    }
}
