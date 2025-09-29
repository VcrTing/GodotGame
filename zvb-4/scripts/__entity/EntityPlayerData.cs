
using System.Collections.Generic;
using ZVB4.Conf;

namespace ZVB4.Entity
{

    [System.Serializable]
    public class EntityPlayerData
    {

        public int CapterFlowerPengNumNow { get; set; } = 2;
        public int EndlessFlowerPengNumNow { get; set; } = 8;

        public int Money { get; set; } = 0;

        public string ShooterNow { get; set; } = PlansConstants.Pea;

        public List<string> ShooterUnlockedList { get; set; } = new List<string>() {
            PlansConstants.Pea,
            PlansConstants.XiguaBing
        };
    }

}