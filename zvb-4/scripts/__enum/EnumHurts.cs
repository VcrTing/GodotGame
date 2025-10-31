namespace ZVB4.Conf
{
    public enum EnumHurts
    {
        Pea = 1, // 普通攻击

        Zheng = 5, // 针刺，无视手中防具，直接作用于头顶

        PaoPao = 8, // 泡泡，无视任何防具，直接作用于身体

        Vip = 3, // 类似于普通攻击

        Cold = 11, // 普通减速

        ColdZheng = 15, // 针刺减速，无视防具，能减速

        IceFreeze = 90,

        Boom = 100,

        Bait = 200,
    }
}
