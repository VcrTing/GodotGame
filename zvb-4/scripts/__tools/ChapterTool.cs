
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
        else if (IsLine(chapter))
        {
            return FolderConstants.WaveSystem + "loader/line_caps_center.tscn";
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
        else if (IsTwoCap(chapter))
        {
            folder = "2_jdnight";
        }
        else if (IsThirdChapter(chapter))
        {
            folder = "3_zsj";
        }
        else if (IsGuanZi(chapter))
        {
            folder = "7_guanzi";
        }
        else if (IsLine(chapter))
        {
            folder = "8_line";
        }
        string jsonPath = $"res://designs/{folder}/cap_{chapter}.json";
        return jsonPath;
    }

    public static bool IsJinDian(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("1");
    }
    public static bool IsTwoCap(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("2");
    }

    public static bool IsGuanZi(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("7");
    }
    public static bool IsLine(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("8");
    }
    public static bool IsThirdChapter(int i)
    {
        string _i = i.ToString();
        return _i.StartsWith("3");
    }

    public static string GetChapterSceneName(int chapter)
    {
        if (IsTwoCap(chapter)) { return "chapter_2jdn.tscn"; }
        if (IsGuanZi(chapter)) { return "chapter_7gz.tscn"; }
        if (IsLine(chapter)) { return "chapter_8line.tscn"; }
        if (IsThirdChapter(chapter)) { return "chapter_3zsj.tscn"; }
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
        else if (chapter == (int)EnumChapter.Seven1) return (int)EnumChapter.Seven2;
        //
        if (chapter == (int)EnumChapter.Seven2) return (int)EnumChapter.One6;
        else if (chapter == (int)EnumChapter.One6) return (int)EnumChapter.One7;
        else if (chapter == (int)EnumChapter.One7) return (int)EnumChapter.One8;
        else if (chapter == (int)EnumChapter.One8) return (int)EnumChapter.One9;
        else if (chapter == (int)EnumChapter.One9) return (int)EnumChapter.Eight1;
        else if (chapter == (int)EnumChapter.Eight1) return (int)EnumChapter.Eight2;
        else if (chapter == (int)EnumChapter.Eight2) return (int)EnumChapter.Two1;

        // 第二章
        if (chapter == (int)EnumChapter.Two1) return (int)EnumChapter.Two2;
        else if (chapter == (int)EnumChapter.Two2) return (int)EnumChapter.Two3;
        else if (chapter == (int)EnumChapter.Two3) return (int)EnumChapter.Two4;
        else if (chapter == (int)EnumChapter.Two4) return (int)EnumChapter.Two5;
        else if (chapter == (int)EnumChapter.Two5) return (int)EnumChapter.Eight3;
        else if (chapter == (int)EnumChapter.Eight3) return (int)EnumChapter.Eight4;
        else if (chapter == (int)EnumChapter.Eight4) return (int)EnumChapter.Two6;
        else if (chapter == (int)EnumChapter.Two6) return (int)EnumChapter.Two7;
        else if (chapter == (int)EnumChapter.Two7) return (int)EnumChapter.Two8;
        else if (chapter == (int)EnumChapter.Two8) return (int)EnumChapter.Two9;

        // 第三章
        if (chapter == (int)EnumChapter.Two8) return (int)EnumChapter.Three1;
        else if (chapter == (int)EnumChapter.Three1) return (int)EnumChapter.Three2;
        else if (chapter == (int)EnumChapter.Three2) return (int)EnumChapter.Three3;
        else if (chapter == (int)EnumChapter.Three3) return (int)EnumChapter.Three4;
        else if (chapter == (int)EnumChapter.Three4) return (int)EnumChapter.Three5;
        //
        else if (chapter == (int)EnumChapter.Three5) return (int)EnumChapter.Three6;
        else if (chapter == (int)EnumChapter.Three6) return (int)EnumChapter.Three7;
        else if (chapter == (int)EnumChapter.Three7) return (int)EnumChapter.Three8;

        // END
        if (chapter == (int)EnumChapter.Three8) return (int)EnumChapter.None;
        return (int)EnumChapter.None;
    }

    public static bool NeedOpenBaseShooterPopup(int num)
    {
        if (num > 2000 && num < 4000)
        {
            return true;
        }
        return false;
    }
}