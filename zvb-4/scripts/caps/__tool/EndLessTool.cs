
using Godot;
using ZVB4.Conf;

public static class EndLessTool
{
    
    public static string EndLessKeyDay = "day";
    public static string EndLessKeyNight = "night";
    
    
    public static string GetChapterSystemLoader(string key)
    {
        return FolderConstants.WaveSystem + "loader/endless_center.tscn";
    }
}