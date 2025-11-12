

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Entity;

public partial class LineCapsCenter : Node2D
{
    private bool _gamingPaused = false;
    private float _gamingTimer = 0f;
    float _gaming = 0f;
    [Export]
    public EnumChapter enumChapter = EnumChapter.One1;
    //
    public static LineCapsCenter Instance { get; private set; }
    private  Dictionary _capData;
    public  Dictionary CapData => _capData;
    //
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

    List<LineGenrateEntity> genrateEntities = new List<LineGenrateEntity>();
    float __genTime = 0f;    
    public override void _Process(double delta)
    {
        if (_gamingPaused) return;
        _gamingTimer += (float)delta;
        _gaming += (float)delta;
        if (_gamingTimer >= 0.5f)
        {
            _gamingTimer = 0f; Gaming();
        }
        if (_gamingTimer == 0) return;
        //
        __genTime += (float)delta;
        if (__genTime >= 0.2f)
        {
            __genTime = 0f;
            GenerateLines();
        }
    }
    private void Gaming()
    {
        if (_gamingPaused) return;
        if (enmys == null) return;
        if (enmys.Count == 0) return;
        WorkForEnmys();
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
        Godot.Collections.Array names = generateInfo["names"].AsGodotArray();
        Godot.Collections.Array grids = generateInfo["grids"].AsGodotArray();
        int isEnmy = 0;
        if (generateInfo.ContainsKey("isenmy")) isEnmy = generateInfo["isenmy"].AsInt32();
        float lazyme = 0f;
        if (generateInfo.ContainsKey("lazyme")) lazyme = generateInfo["lazyme"].AsSingle();
        int loopNum = 0;
        if (generateInfo.ContainsKey("loop")) loopNum = generateInfo["loop"].AsInt32();
        float loopDelay = 0f;
        if (generateInfo.ContainsKey("loopdelay")) loopDelay = generateInfo["loopdelay"].AsSingle();

        if (names.Count == 0 || grids.Count == 0) return;
        for (int loopIndex = 0; loopIndex < loopNum; loopIndex++)
        {
            for (int i = 0; i < names.Count; i++)
            {
                string objName = names[i].AsString();
                int lineNumY = 1;
                int lineNumX = grids[i].AsInt32();
                double genTime = _gaming + (loopDelay * loopIndex) + (i * lazyme) + 0.5f;
                LineGenrateEntity genrateEntity = new LineGenrateEntity(objName, 1, lineNumX, lineNumY, isEnmy == 1, genTime);
                genrateEntities.Add(genrateEntity);
            }
        }
    }
    
    void GenerateLines()
    {
        foreach (LineGenrateEntity entity in genrateEntities)
        {
            if (_gaming >= entity.genTime)
            {
                // 生成敌人逻辑
                bool isEnmy = entity.isEnmy;
                if (isEnmy)
                {
                    EnmyGenerator.Instance.GenerateEnemyByCode(entity.objName, entity.lineNumX, 0, 0);
                }
                else
                {
                    // PlantGenerator.Instance?.GenerateLinePlants(entity.objName, entity.copyNum, entity.lineNumX, entity.lineNumY);
                }
            }
        }
        // 移除已经生成的实体
        genrateEntities.RemoveAll(e => _gaming >= e.genTime);
    }
    public void PauseGaming() => _gamingPaused = true;
    public void ResumeGaming() => _gamingPaused = false;
    Dictionary enmyswaveflag;
    Dictionary enmys;
    Dictionary suns;
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
                LoadFreezeCard(_capData);
            }
        }
    }
    void LoadMiao(Dictionary _capData)
    {
        string initmiaomode = _capData["initmiaomode"].AsString();
        if (initmiaomode != null && initmiaomode != "" && initmiaomode != "none")
        {
            string initmiaorandomnummode = _capData["initmiaorandomnummode"].AsString();
            Godot.Collections.Array initmiaolist = _capData["initmiaolist"].AsGodotArray();
            //
            var miaoCenter = RewordMiaoCenterSystem.Instance;
            if (miaoCenter != null)
            {
                miaoCenter.DumpInitPlansMiao(initmiaomode, initmiaorandomnummode, initmiaolist);
            }
            Godot.Collections.Array allowmiaolist = _capData["allowmiaolist"].AsGodotArray();
            miaoCenter?.SetAllowMiaoList(allowmiaolist);
        }
        //
    }
    async void LoadInitShooter(Dictionary varData)
    {
        string initshooter = _capData["initshooter"].AsString();
        if (initshooter == null || initshooter == "_")
        {
            if (SaveDataManager.Instance == null)
            {
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
    
    void LoadZombis(Dictionary _capData)
    {
        if (_capData.ContainsKey("enmys"))
        {
            var enmysVariant = _capData["enmys"].AsGodotDictionary();
            if (enmysVariant is Dictionary)
            {
                // 加入僵尸数据
                enmys = (Dictionary)enmysVariant;
                // 设置默认缩放
                EnmyGenerator.SetInitScale(
                    (float)_capData["enmyspeedmovescale"],
                    (float)_capData["enmybehurtscale"],
                    (float)_capData["enmyviewscale"], 
                    (float)_capData["enmyspeedattackscale"]
                    );
            }
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
    
    // 生成 card 
    void LoadFreezeCard(Dictionary varData)
    {
        Godot.Collections.Array buffs = varData["cards"].AsGodotArray();
        if (buffs == null || buffs.Count <= 0) return;
        foreach (var v in buffs)
        {
            var buffInfo = v.AsGodotDictionary();
            LineFreezeCardSystem.Instance?.CreateFreezeCard(buffInfo);
        }
    }
}
