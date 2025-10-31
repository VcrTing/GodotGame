
using System.Collections.Generic;
using ZVB4.Conf;

namespace ZVB4.Entity
{

    [System.Serializable]
    public class EntityPlayerData
    {
        public EnumChapter ChapterNow { get; set; } = EnumChapter.One1;

        public int CapterFlowerPengNumNow { get; set; } = 6;
        public int EndlessFlowerPengNumNow { get; set; } = 8;

        public int Money { get; set; } = 0;

        public string ShooterNow { get; set; } = PlansConstants.Pea;
        public string ShooterBaseLast { get; set; } = PlansConstants.Pea;

        public string PlansUnLock { get; set; } = "_" + PlansConstants.Pea + "_" + "_" + PlansConstants.SunFlower + "_";
        public string ShooterUnLimit { get; set; } = "_" + PlansConstants.Pea + "_";
    }

}