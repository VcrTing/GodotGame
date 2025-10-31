namespace ZVB4.Conf
{
    public enum EnumMoveType
    {
        Stone = 0,

        // 直线步行
        LineWalk = 1,
        WalkToPalyer = 11,
        // 间歇性步行
        LineWalkFast = 2,
        // 直线跑步
        LineRun = 3,
        // 间歇性跑步
        RunToPalyer = 31,
        // 直线加速跑步
        LineRunFast = 4,
        // 间歇性加速跑步
        IntermittentRunFast = 41,

    }
}
