
namespace ZVB4.Conf
{
    public static class RewordConstants
    {
        public const string Sun = "Sun";
        public const string Gift = "Gift";
        public const string Money = "Money";
        public const string Baoshi = "Baoshi";

        // 
        public static readonly System.Collections.Generic.Dictionary<string, string> RewordSceneDict = new System.Collections.Generic.Dictionary<string, string>
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

    }

}