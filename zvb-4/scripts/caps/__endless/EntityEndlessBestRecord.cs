
using System.Collections.Generic;
using ZVB4.Conf;

namespace ZVB4.Entity
{

    [System.Serializable]
    public class EntityEndlessBestRecord
    {
        public int EnmyWave { get; set; } = 0;
        public int SunTotalGet { get; set; } = 0;
        public int MoneyTotalGet { get; set; } = 0;
        public string Timed { get; set; } = "";
    }

}