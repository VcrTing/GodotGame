

using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Entity;

public partial class CapsCenter : Node2D
{
    private bool _gamingPaused = false;
    private float _gamingTimer = 0f;
    float _gaming = 0f;
    [Export]
    public EnumChapter enumChapter = EnumChapter.One1;

    public static CapsCenter Instance { get; private set; }

    private Godot.Collections.Dictionary _capData;

    public Godot.Collections.Dictionary CapData => _capData;

    EntityGameRunnerData gameRunnerData = null;

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
            GD.Print("LoadGame => SaveGamerRunnerDataManger.Instance: " + (ins != null));
            GetTree().CreateTimer(0.1f).Timeout += () => LoadGame();
            return;
        }
        gameRunnerData = ins.GetGameRunnerData();
        CapterNumber = ins.GetCapterNumber();
        GD.Print("加载章节 => CapterNumber: " + CapterNumber);
        string jsonPath = $"res://designs/jindian/cap_{CapterNumber}.json";
        LoadCapData(jsonPath);
    }

    public override void _Process(double delta)
    {
        if (_gamingPaused) return;
        _gamingTimer += (float)delta;
        _gaming += (float)delta;
        if (_gamingTimer >= 0.5f)
        {
            _gamingTimer = 0f;
            Gaming();
        }
    }

    private void Gaming()
    {
        if (_gamingPaused) return;
        WorkForEnmys();
    }

    string workEnmysString = "";
    // 敌人工作器
    void WorkForEnmys()
    {
        if (enmys == null) return;
        if (enmys.Count == 0) return;
        // 取enmys数据
        foreach (string key in enmys.Keys)
        {
            // 去掉已经执行过的 key
            if (workEnmysString.Contains(key + "__"))
            {
                continue;
            }
            // key为秒数，value为敌人信息
            if (float.TryParse(key, out float sec))
            {
                if (_gaming >= sec)
                {
                    workEnmysString += (key + "__");
                    // 生成敌人方法（示例）
                    Godot.Collections.Array enmyInfo = enmys[key].AsGodotArray();
                    for (int i = 0; i < enmyInfo.Count; i++)
                    {
                        var info = (Godot.Collections.Dictionary)enmyInfo[i];
                        if (info != null)
                        {
                            WorkForGenerateEnmys(info);
                        }
                    }
                }
            }
        }
    }
    void WorkForGenerateEnmys(Godot.Collections.Dictionary generateInfo)
    {

        Godot.Collections.Array types = generateInfo["types"].AsGodotArray();
        Godot.Collections.Array generator = generateInfo["generator"].AsGodotArray();
        // 
        string typesmode = generateInfo["typesmode"].AsString();
        string generatormode = generateInfo["generatormode"].AsString();
        float lazyme = 0f;
        if (generateInfo.ContainsKey("lazyme"))
        {
            lazyme = generateInfo["lazyme"].AsSingle();
        }

        // 
        EnmyGenerator.GenerateEnemiesByConfig(types, generator, typesmode, generatormode, lazyme);
    }

    public void PauseGaming() => _gamingPaused = true;
    public void ResumeGaming() => _gamingPaused = false;


    Godot.Collections.Dictionary enmyswaveflag;
    Godot.Collections.Dictionary enmys;
    Godot.Collections.Dictionary suns;
    void LoadCapData(string jsonPath)
    {
        if (Godot.FileAccess.FileExists(jsonPath))
        {
            using var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
            var json = file.GetAsText();
            var result = Json.ParseString(json);
            if (result.VariantType == Variant.Type.Dictionary)
            {
                _capData = (Godot.Collections.Dictionary)result;
                LoadZombis(_capData);
                if (_capData.ContainsKey("suns"))
                {
                    var sunsVariant = _capData["suns"].AsGodotDictionary();
                    if (sunsVariant is Godot.Collections.Dictionary)
                    {
                        suns = (Godot.Collections.Dictionary)sunsVariant;
                    }
                }
                if (_capData.ContainsKey("enmyswaveflag"))
                {
                    var enmyswaveflagVariant = _capData["enmyswaveflag"];
                    if (enmyswaveflagVariant is Godot.Collections.Dictionary)
                    {
                        enmyswaveflag = (Godot.Collections.Dictionary)enmyswaveflagVariant;
                    }
                }
                //
                LoadVar(_capData);
            }
            else
                GD.PrintErr($"Failed to parse dictionary from {jsonPath}");
        }
    }

    void LoadZombis(Godot.Collections.Dictionary _capData)
    {
        if (_capData.ContainsKey("enmys"))
        {
            var enmysVariant = _capData["enmys"].AsGodotDictionary();
            if (enmysVariant is Godot.Collections.Dictionary)
            {
                // 加入僵尸数据
                enmys = (Godot.Collections.Dictionary)enmysVariant;
                // 计算全部僵尸数量
                int totalZombiCount = 0;
                foreach (string key in enmys.Keys)
                {
                    Godot.Collections.Array enmyInfo = enmys[key].AsGodotArray();
                    foreach (var item in enmyInfo)
                    {
                        var info = (Godot.Collections.Dictionary)item;
                        if (info != null && info.ContainsKey("types"))
                        {
                            int count = info["types"].AsGodotArray().Count;
                            totalZombiCount += count;
                        }
                    }
                }
                GameStatistic.Instance?.SetZombieChapterTotal(totalZombiCount);
            }
        }
    }

    void LoadVar(Godot.Collections.Dictionary varData)
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
            // 多加 10s
            GameWinnerChecker.Instance?.AddTimePoint(gamechecktime + 10f);
            // 多加 20s
            GameWinnerChecker.Instance?.AddTimePoint(gamechecktime + 20f);
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
