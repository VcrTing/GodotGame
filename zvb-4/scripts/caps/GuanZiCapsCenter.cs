using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class GuanZiCapsCenter : Node2D
{
    
    private bool _gamingPaused = false;
    private float _gamingTimer = 0f;
    float _gaming = 0f;
    [Export]
    public EnumChapter enumChapter = EnumChapter.One1;

    public static GuanZiCapsCenter Instance { get; private set; }
    private  Dictionary _capData;
    public  Dictionary CapData => _capData;

    public override void _Ready()
    {
        Instance = this;
        LoadGame();
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
        GD.Print("LoadCapData => jsonPath: " + jsonPath);
        if (Godot.FileAccess.FileExists(jsonPath))
        {
            using var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
            var json = file.GetAsText();
            var result = Json.ParseString(json);
            if (result.VariantType == Variant.Type.Dictionary)
            {
                _capData = (Dictionary)result;
                LoadVar(_capData);
                LoadGuanZi(_capData);
                LoadFlowerPeng(_capData);
            }
            else
                GD.PrintErr($"Failed to parse dictionary from {jsonPath}");
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
    
    
    void LoadFlowerPeng( Dictionary _capData)
    {
        int initpeng = _capData["initpeng"].AsInt32();
        var flowers = FlowerPengSystem.Instance;
        if (flowers != null)
        {
            flowers.Init(initpeng);
        }
    }
    void LoadVar( Dictionary varData)
    {
        if (varData.ContainsKey("initsun"))
        {
            // 初始阳光
            int initsun = (int)varData["initsun"];
            // 设置阳光
            LoadInitSun(initsun);
        }
        if (varData.ContainsKey("gamechecktime"))
        {
            // 游戏检测时间
            float gamechecktime = (float)varData["gamechecktime"];
            GameWinnerChecker.Instance?.AddTimePoint(gamechecktime);
        }
    }
    void LoadInitSun(int v)
    {
        var suns = SunCenterSystem.Instance;
        if (suns == null)
        {
            GD.PrintErr("LoadInitSun => SunCenterSystem.Instance is null");
            return;
        }
        suns?.SetValue(v);
    }
}
