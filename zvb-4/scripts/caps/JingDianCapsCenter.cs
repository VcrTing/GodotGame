

using Godot;
using ZVB4.Conf;
using Godot.Collections;

public partial class JingDianCapsCenter : Node2D
{
    private bool _gamingPaused = false;
    private float _gamingTimer = 0f;
    float _gaming = 0f;
    [Export]
    public EnumChapter enumChapter = EnumChapter.One1;
    //
    public static JingDianCapsCenter Instance { get; private set; }
    private  Dictionary _capData;
    public  Dictionary CapData => _capData;
    //
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
        GD.Print("加载经典章节数据，章节编号：" + CapterNumber);
        LoadCapData(ChapterTool.GetChapterJsonFilePath(CapterNumber));
    }
    public override void _Process(double delta)
    {
        if (_gamingPaused) return;
        _gamingTimer += (float)delta;
        _gaming += (float)delta;
        if (_gamingTimer >= 0.5f)
        {
            _gamingTimer = 0f; Gaming();
        }
    }
    private void Gaming()
    {
        if (_gamingPaused) return;
        if (enmys == null) return;
        if (enmys.Count == 0) return;
        WorkForEnmys();
        WorkForAttackStart();
    }
    string workEnmysString = "";
    // 敌人工作器
    void WorkForEnmys()
    {
        // 取enmys数据
        foreach (string key in enmys.Keys)
        {
            // 去掉已经执行过的 key
            if (workEnmysString.Contains(key + "__")) continue;
            if (float.TryParse(key, out float sec))
            {
                if (_gaming >= sec)
                {
                    workEnmysString += (key + "__");
                    // 生成敌人方法（示例）
                    Godot.Collections.Array enmyInfo = enmys[key].AsGodotArray();
                    for (int i = 0; i < enmyInfo.Count; i++)
                    {
                        var info = ( Dictionary)enmyInfo[i];
                        if (info != null)
                        {
                            WorkForGenerateEnmys(info);
                        }
                    }
                }
            }
        }
    }
    void WorkForGenerateEnmys(Dictionary generateInfo)
    {
        Godot.Collections.Array types = generateInfo["types"].AsGodotArray();
        Godot.Collections.Array generator = generateInfo["generator"].AsGodotArray();
        string typesmode = generateInfo["typesmode"].AsString();
        string generatormode = generateInfo["generatormode"].AsString();
        float lazyme = 0f;
        if (generateInfo.ContainsKey("lazyme"))
        {
            lazyme = generateInfo["lazyme"].AsSingle();
        }
        int randomxrate = 0;
        if (generateInfo.ContainsKey("randomxrate"))
        {
            randomxrate = generateInfo["randomxrate"].AsInt32();
        }
        float redeyeratio = 0f;
        if (generateInfo.ContainsKey("redeyeratio"))
        {
            redeyeratio = generateInfo["redeyeratio"].AsSingle();
        }
        // 通过配置生成敌人
        EnmyGenerator.GenerateEnemiesByConfig(types, generator, typesmode, generatormode, lazyme, randomxrate, redeyeratio);
    }

    // 咆哮
    string workStartsString = "";
    void WorkForAttackStart()
    {
        if (attackstarts != null && attackstarts.Count > 0)
        {
            foreach (var item in attackstarts)
            {
                if (workStartsString.Contains(item + "__")) continue;
                if (Mathf.Abs(_gaming - (float)item) < 1f)
                {
                    workStartsString += (item + "__");
                    // 播放咆哮音效
                    SoundOneshotController.Instance?.PlayFx("Zombi/wavestart/start_attack_night", "attack_star", 3, 1f, new Vector2(0, 0));
                }
            }
        }
    }
    public void PauseGaming() => _gamingPaused = true;
    public void ResumeGaming() => _gamingPaused = false;
    Godot.Collections.Array attackstarts;
    Dictionary enmys;
    void LoadCapData(string jsonPath)
    {
        if (Godot.FileAccess.FileExists(jsonPath))
        {
            using var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
            var result = Json.ParseString(file.GetAsText());
            if (result.VariantType == Variant.Type.Dictionary)
            {
                _capData = ( Dictionary)result;
                LoadVar(_capData);
                LoadMiao(_capData);
                LoadZombis(_capData);
                LoadInitShooter(_capData);
            }
        }
    }
    void LoadMiao( Dictionary _capData)
    {
        string initmiaomode = _capData["initmiaomode"].AsString();
        if (initmiaomode != null && initmiaomode != "" && initmiaomode != "none")
        {
            string initmiaorandomnummode = _capData["initmiaorandomnummode"].AsString();
            Godot.Collections.Array initmiaolist = _capData["initmiaolist"].AsGodotArray();
            //
            // string initshooter = _capData["initshooter"].AsString();
            // initmiaolist.Add(initshooter);
            //
            var miaoCenter = RewordMiaoCenterSystem.Instance;
            if (miaoCenter != null)
            {
                miaoCenter.DumpInitPlansMiao(initmiaomode, initmiaorandomnummode, initmiaolist);
            }
            Godot.Collections.Array allowmiaolist = _capData["allowmiaolist"].AsGodotArray();
            miaoCenter?.SetAllowMiaoList(allowmiaolist);
        }
    }
    void LoadZombis(Dictionary _capData)
    {
        if (_capData.ContainsKey("enmys"))
        {
            var enmysVariant = _capData["enmys"].AsGodotDictionary();
            if (enmysVariant is Dictionary)
            {
                // 加入僵尸数据
                enmys = (Dictionary)enmysVariant;
                // 计算全部僵尸数量
                int totalZombiCount = 0;
                foreach (string key in enmys.Keys)
                {
                    Godot.Collections.Array enmyInfo = enmys[key].AsGodotArray();
                    foreach (var item in enmyInfo)
                    {
                        var info = (Dictionary)item;
                        if (info != null && info.ContainsKey("types"))
                        {
                            int count = info["types"].AsGodotArray().Count;
                            totalZombiCount += count;
                        }
                    }
                }
                GameStatistic.Instance?.SetZombieChapterTotal(totalZombiCount);
                // 设置默认缩放
                EnmyGenerator.SetInitScale(
                    (float)_capData["enmyspeedmovescale"],
                    (float)_capData["enmybehurtscale"],
                    (float)_capData["enmyviewscale"],
                    (float)_capData["enmyspeedattackscale"]
                    );
            }
        }
        if (_capData.ContainsKey("attackstarts"))
        {
            attackstarts = _capData["attackstarts"].AsGodotArray();
        }
    }
    void LoadVar(Dictionary varData)
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
    }
    
    async void LoadInitShooter(Dictionary varData)
    {
        string initshooter = _capData["initshooter"].AsString();
        if (initshooter == null || initshooter == "_") {
            if (SaveDataManager.Instance == null) {
                GetTree().CreateTimer(0.1f).Timeout += () => LoadInitShooter(_capData);
                return;
            }
            initshooter = SaveDataManager.Instance.GetPlayerShooter();
        }
        if (!PlayerController.CheckAlive())
        {
            GetTree().CreateTimer(0.1f).Timeout += () => LoadInitShooter(_capData);
            return;
        }
        // 玩家初始化参数
        if (varData.ContainsKey("shooterlowestattackspeedratio"))
        {
            float lowestSpeedRatio = varData["shooterlowestattackspeedratio"].AsSingle();
            PlayerController.Instance?.SetLowestAttackSpeedRatio(lowestSpeedRatio);
        }
        if (varData.ContainsKey("shootershootratio"))
        {
            float shootRatio = varData["shootershootratio"].AsSingle();
            PlayerController.Instance?.SetShootRatio(shootRatio);
        }
        PlayerController.Instance?.LoadInitShooter(initshooter);
    }
}
