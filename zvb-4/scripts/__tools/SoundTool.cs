using Godot;
using ZVB4.Conf;

namespace ZVB4.Tool
{
    public static class SoundTool
    {

        public const string SoundBusUx = "Ux";
        
        /// <summary>
        /// 生成带随机index的音效路径
        /// </summary>
        /// <param name="folderPath">如"fx"</param>
        /// <param name="soundName">如"hit"</param>
        /// <param name="soundIndex">最大音效编号</param>
        /// <returns>完整音效路径</returns>
        public static string GetRandomSoundPath(string folderPath, string soundName, int soundIndex)
        {
            int randomIndex = (soundIndex > 1) ? GD.RandRange(1, soundIndex) : 1;
            return $"{FolderConstants.Musics}{folderPath.TrimEnd('/')}/{soundName}_{randomIndex}.mp3";
        }

        
        public static float CalcPanByPos(Vector2 pos)
        {
            float x = pos.X;
            return CalcPanByX(x);
        }
        public static float CalcPanByX(float x)
        {
            if (x <= -GameContants.ScreenHalfW) return -1f;
            if (x >= GameContants.ScreenHalfW) return 1f;
            // KKK
            float v = x / (GameContants.ScreenHalfW - 30f);
            if (v > -0.1f && v < 0.1f)
            {
                if (v < 0) v = -0.1f;
                else v = 0.1f;
            }
            return v;
        }
    }
}
