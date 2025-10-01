
using ZVB4.Conf;

public static class ChapterTool
{

    public static string MainMenuScene = "main.tscn";
    public static string MainNextScene = "main_next.tscn";

    public static string GetNextChapterSceneName(int chapter)
    {
        string i = chapter.ToString();
        if (i.StartsWith("1"))
        {
            return "chapter_1xx.tscn";
        }
        return "chapter_1xx.tscn";
    }

    public static int GetNextChapterNum(int chapter)
    {
        if (chapter == (int)EnumChapter.One1) return (int)EnumChapter.One2;
        if (chapter == (int)EnumChapter.One2) return (int)EnumChapter.One3;
        if (chapter == (int)EnumChapter.One3) return (int)EnumChapter.One4;
        if (chapter == (int)EnumChapter.One4) return (int)EnumChapter.None;
        return (int)EnumChapter.None;
    }
}