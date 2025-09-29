using Godot;
using ZVB4.Conf;

namespace ZVB4.Tool
{
    public static class SoundTool
    {
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

        
        public static float CalcPanByX(float x)
        {
            if (x <= -GameContants.ScreenHalfW) return -1f;
            if (x >= GameContants.ScreenHalfW) return 1f;
            return x / GameContants.ScreenHalfW;
        }
    }
}
