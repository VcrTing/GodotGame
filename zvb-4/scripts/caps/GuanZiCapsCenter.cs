using Godot;
using Godot.Collections;
using ZVB4.Conf;

public partial class GuanZiCapsCenter : Node2D
{
    public static GuanZiCapsCenter Instance { get; private set; }
    private  Dictionary _capData;
    public override void _Ready()
    {
        Instance = this;
        CallDeferred(nameof(LoadGame));
    }
    public int CapterNumber = (int) EnumChapter.One1;
    void LoadGame()
    {
        var ins = SaveGamerRunnerDataManger.Instance;
        if (ins == null)
        {
            GetTree().CreateTimer(0.1f).Timeout += () => LoadGame();
            return;
        }
        CapterNumber = ins.GetCapterNumber();
        LoadCapData(ChapterTool.GetChapterJsonFilePath(CapterNumber));
    }
    void LoadCapData(string jsonPath)
    {
        if (Godot.FileAccess.FileExists(jsonPath))
        {
            using var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
            var result = Json.ParseString(file.GetAsText());
            if (result.VariantType == Variant.Type.Dictionary)
            {
                _capData = (Dictionary)result;
                LoadVar(_capData);
                LoadGuanZi(_capData);
            }
        }
    }
    void LoadGuanZi(Dictionary _capData)
    {
        if (_capData.ContainsKey("guanzis"))
        {
            Godot.Collections.Array guanziList = _capData["guanzis"].AsGodotArray();
            if (guanziList != null)
            {
                foreach (var item in guanziList)
                {
                    Godot.Collections.Dictionary data = item.AsGodotDictionary();
                    if (data != null)
                    {
                        int gezi = (int)data["gezi"];
                        int v = (int)data["view"];
                        Godot.Collections.Array enmyrandomlist = data["enmyrandomlist"].AsGodotArray();
                        Godot.Collections.Array plansrandomlist = data["plansrandomlist"].AsGodotArray();
                        float mustenmyratio = (float)data["mustenmyratio"];
                        // 生成
                        GuanZiCenterSystem.Instance.GenerateGuanZi(gezi, v, mustenmyratio, plansrandomlist, enmyrandomlist);
                    }
                }
            }
        }
    }
    void LoadVar( Dictionary varData)
    {
        if (varData.ContainsKey("initsun"))
        {
            // 初始阳光
            int initsun = (int)varData["initsun"];
            SunCenterSystem.Instance?.SetValue(initsun);
        }
        if (varData.ContainsKey("gamechecktime"))
        {
            // 游戏检测时间
            float gamechecktime = (float)varData["gamechecktime"];
            GameWinnerChecker.Instance?.AddTimePoint(gamechecktime);
        }
        if (varData.ContainsKey("initpeng"))
        {
            // 加载花盆
            int initpeng = _capData["initpeng"].AsInt32();
            FlowerPengSystem.Instance?.Init(initpeng);
        }
        if (varData.ContainsKey("generatemiaoratio"))
        {
            // 生成苗几率
            float generatemiaoratio = varData["generatemiaoratio"].AsSingle();
            if (RewordMiaoCenterSystem.Instance != null)
            {
                RewordMiaoCenterSystem.Instance?.SetGenerateRatio(generatemiaoratio);
            }
        }
        if (varData.ContainsKey("shootershootratio"))
        {
            // 射手射击数量几率
            float shootershootratio = varData["shootershootratio"].AsSingle();
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance?.SetShootRatio(shootershootratio);
            }
        }
    }
}
