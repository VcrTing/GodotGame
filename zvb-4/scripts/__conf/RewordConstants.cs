
using System.Collections.Generic;

namespace ZVB4.Conf
{
    public static class RewordConstants
    {
        public const string Sun = "Sun";
        public const string Gift = "Gift";
        public const string Money = "Money";
        public const string Baoshi = "Baoshi";

        // 
        public static readonly Dictionary<string, string> RewordSceneDict = new Dictionary<string, string>
        {
            { Sun, FolderConstants.WaveObj + "sun.tscn" },
        };

        // 根据key获取场景路径
        public static string GetRewordGroupScene(string key)
        {
            return FolderConstants.WaveObj + "reword_group.tscn";
        }
        public static string GetRewordScene(string key)
        {
            if (RewordSceneDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }

        // 植物随机加权值
        public static readonly Dictionary<string, int> PlansRewordWeightDict = new Dictionary<string, int>
        {
            { PlansConstants.Pea, 10 },
            { PlansConstants.XiguaBing, 5 },
            { PlansConstants.SunFlower, 30 },
            { PlansConstants.Cherry, 10 },
        };

        public static int GetPlansRewordWeight(string key)
        {
            if (PlansRewordWeightDict.TryGetValue(key, out var value))
                return value;
            return 0;
        }

    }

}