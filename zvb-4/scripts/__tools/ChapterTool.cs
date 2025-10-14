
using Godot;
using ZVB4.Conf;

public static class ChapterTool
{

    public static string MainMenuScene = "main.tscn";
    public static string MainNextScene = "main_next.tscn";

    public static string GetChapterSystemLoader(int chapter)
    {
        if (IsGuanZi(chapter))
        {
            return FolderConstants.WaveSystem + "loader/guan_zi_caps_center.tscn";
        }
        return FolderConstants.WaveSystem + "loader/jing_dian_caps_center.tscn";
    }

    public static string GetChapterJsonFilePath(int chapter)
    {
        string folder = "jindian";
        if (IsJinDian(chapter))
        {
            folder = "1_jindian";
        }
        else if (IsGuanZi(chapter))
        {
            folder = "7_guanzi";
        }
        else
        {
            
        }
        string jsonPath = $"res://designs/{folder}/cap_{chapter}.json";
        return jsonPath;
    }

    public static bool IsJinDian(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("1");
    }
    public static bool IsGuanZi(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("7");
    }

    public static string GetChapterSceneName(int chapter)
    {
        if (IsJinDian(chapter))
        {
            return "chapter_1xx.tscn";
        }
        if (IsGuanZi(chapter))
        {
            return "chapter_7gz.tscn";
        }
        return "chapter_1xx.tscn";
    }

    public static int GetNextChapterNum(int chapter)
    {
        // 第一章
        if (chapter == (int)EnumChapter.One1) return (int)EnumChapter.One2;
        else if (chapter == (int)EnumChapter.One2) return (int)EnumChapter.One3;
        else if (chapter == (int)EnumChapter.One3) return (int)EnumChapter.One4;
        else if (chapter == (int)EnumChapter.One4) return (int)EnumChapter.One5;
        else if (chapter == (int)EnumChapter.One5) return (int)EnumChapter.Seven1;
        else if (chapter == (int)EnumChapter.Seven1) return (int)EnumChapter.One6;
        else if (chapter == (int)EnumChapter.One6) return (int)EnumChapter.One7;
        else if (chapter == (int)EnumChapter.One7) return (int)EnumChapter.One8;

        if (chapter == (int)EnumChapter.One8) return (int)EnumChapter.One9;
        else if (chapter == (int)EnumChapter.One9) return (int)EnumChapter.One10;
        else if (chapter == (int)EnumChapter.One10) return (int)EnumChapter.One11;
        else if (chapter == (int)EnumChapter.One11) return (int)EnumChapter.One12;
        else if (chapter == (int)EnumChapter.One12) return (int)EnumChapter.One13;

        // END
        if (chapter == (int)EnumChapter.One13) return (int)EnumChapter.None;
        return (int)EnumChapter.None;
    }
}