namespace ZVB4.Conf
{
    public enum EnumMoveType
    {
        Stone = 0,

        // 直线步行
        LineWalk = 1,
        // 间歇性步行
        LineWalkFast = 11,
        // 直线跑步
        LineRun = 2,
        // 间歇性跑步
        RunToPalyer = 21,
        // 直线加速跑步
        LineRunFast = 3,
        // 间歇性加速跑步
        IntermittentRunFast = 31,

    }
}
