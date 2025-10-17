

using System.Collections.Generic;
using ZVB4.Conf;

namespace ZVB4.Entity
{

    [System.Serializable]
    public class LineGenrateEntity
    {
        public string objName;
        public int copyNum;
        public int lineNumX;
        public int lineNumY;
        public bool isEnmy;
        public double genTime;

        public LineGenrateEntity(string objName, int copyNum, int lineNumX, int lineNumY, bool isEnmy, double genTime)
        {
            this.objName = objName;
            this.copyNum = copyNum;
            this.lineNumX = lineNumX;
            this.lineNumY = lineNumY;
            this.isEnmy = isEnmy;
            this.genTime = genTime;
        }
    }
}