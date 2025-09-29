

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

    EntityPlayerData playerData = null;

    public override void _Ready()
    {
        Instance = this;
    }

    void LoadGame()
    {
        playerData = SaveDataManager.Instance.GetPlayerData();
        
        string jsonPath = $"res://designs/jindian/cap_{(int)enumChapter}.json";
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
                if (_capData.ContainsKey("enmys"))
                {
                    var enmysVariant = _capData["enmys"].AsGodotDictionary();
                    if (enmysVariant is Godot.Collections.Dictionary)
                    {
                        enmys = (Godot.Collections.Dictionary)enmysVariant;
                    }
                }
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
            }
            else
                GD.PrintErr($"Failed to parse dictionary from {jsonPath}");
        }
        else
        {
            GD.PrintErr($"File not found: {jsonPath}");
        }
    }
}
