
using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class EnmyGenerator : Node2D
{
    public static EnmyGenerator Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        XPointList.Clear();
        XPointList.AddRange(GetTenIntervalsX());
        YPointList.Clear();
        YPointList.AddRange(GetTenIntervalsY());
    }
    [Export]
    public float TileMapW = GameContants.ZombieTileW; // 每个格子的宽度 
    [Export]
    public int TileNumHalf = GameContants.GrassNumHalf;

    List<Vector2> XPointList = new List<Vector2>();
    List<Vector2> YPointList = new List<Vector2>();
    /// <summary>
    /// 获取以本节点x轴为中心的10份区间坐标点（共10个，左5右5）
    /// </summary>
    /// <returns>长度为10的Vector2数组</returns>
    /// 
    /// 
    public Vector2[] GetTenIntervalsX()
    {
        int num = TileNumHalf * 2;
        Vector2[] points = new Vector2[num];
        float centerX = this.Position.X;
        float y = this.Position.Y;
        for (int i = 0; i < num; i++)
        {
            float x = centerX - TileMapW * TileNumHalf + TileMapW * i;
            points[i] = new Vector2(x, y);
        }
        return points;
    }
    public Vector2[] GetTenIntervalsY()
    {
        int num = TileNumHalf * 2;
        Vector2[] points = new Vector2[num];
        float x = this.Position.X - GameContants.ScreenHalfW - GameContants.ScreenHalfW;
        //
        float everyY = ((HorizonYEnmyGenEnd - (TileMapW * 1.5f)) - (HorizonYEnmyGen)) / num;
        //
        for (int i = 0; i < num; i++)
        {
            float __y = (HorizonYEnmyGen + TileMapW) + (everyY * i);
            points[i] = new Vector2(x, __y);
        }
        return points;
    }

    async void Generate(string type, int grid, string genmode, float lazyme, int randomXRate, float redeyeratio)
    {
        if (lazyme > 0)
        {
            await ToSignal(GetTree().CreateTimer(lazyme), "timeout");
        }
        if (genmode == "random")
        {
            Instance.GenerateEnemyByCode(type, randomXRate, redeyeratio);
        }
        else if (genmode == "next")
        {
            Instance.GenerateEnemyByCode(type, grid, randomXRate, redeyeratio);
        }
        // fary
        else
        {
            GenerateEnemyByCodeY(type, grid, redeyeratio);
        }
    }

    [Export]
    public float InitMoveSpeedScale = 1f;
    [Export]
    public float InitBeHurtScale = 1f;
    [Export]
    public float InitViewScale = 1f;
    [Export]
    public float InitAttackSpeedScale = 1f;

    public static void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale)
    {
        if (Instance == null)
        {
            return;
        }
        Instance.InitMoveSpeedScale = movespeedscale;
        Instance.InitBeHurtScale = behurtscale;
        Instance.InitViewScale = viewscale; 
        Instance.InitAttackSpeedScale = attackspeedscale;
    }
    /// <summary>

    /// 根据types、generator、typesmode、generatormode批量生成敌人
    /// </summary>
    public static void GenerateEnemiesByConfig(Godot.Collections.Array types, Godot.Collections.Array generator, string typesmode, string generatormode, float lazyme, int randomXRate, float redeyeratio)
    {
        var gen = Instance;
        if (gen == null)
        {
            return;
        }
        if (typesmode == "next")
        {
            int count = Math.Min(types.Count, generator.Count);
            for (int i = 0; i < count; i++)
            {
                gen.Generate(types[i].AsString(), (int)generator[i], generatormode, lazyme * i, randomXRate, redeyeratio);
            }
        }
        if (typesmode == "random")
        {
            if (types.Count > 0 && generator.Count > 0)
            {
                int count = Math.Min(types.Count, generator.Count);
                var idxList = CommonTool.GetShuffledIndexList(count);
                int i = 0;
                foreach (var idx in idxList)
                {
                    i += 1;
                    gen.Generate(types[idx].AsString(), (int)generator[idx], generatormode, lazyme * i, randomXRate, redeyeratio);
                }
            }
        }
    }
    
    public Node2D GenerateEnemyByCode(string code, int randomXRate, float redeyeratio)
    {
        int max = TileNumHalf * 2;
        int tileIndex = (int)GD.RandRange(1, max);
        return GenerateEnemyByCode(code, tileIndex, randomXRate, redeyeratio);
    }
    float DoRandomX(Vector2 tilePos, int randomXRate)
    {
        float v = tilePos.X + TileMapW / 2f; 
        if (randomXRate <= 0)
        {
            return v;
        }
        float left = v - TileMapW / 2f;
        float right = v + TileMapW / 2f;
        float randX = (float)GD.RandRange(left, right);
        return randX;
    }

    public Node2D GenerateEnemyByCode(string name, int tileIndex, int randomXRate, float redeyeratio)
    {
        try
        {
            string path = EnmyTypeConstans.GetZombiWrapperScenePath(name);
            var packed = GD.Load<PackedScene>(path);
            if (packed == null) return null;
            var instance = packed.Instantiate<Node2D>();
            if (tileIndex < 1 || tileIndex > XPointList.Count) return instance;
            var tilePos = XPointList[tileIndex - 1];
            var pos = instance.Position;
            pos.X = DoRandomX(tilePos, randomXRate);
            pos.Y = pos.Y + HorizonYEnmyGen;
            return Doing(instance, pos, name, redeyeratio);
        }
        catch (Exception ex) { } return null;
    }
    public Node2D GenerateEnemyByCodeY(string name, int tileIndex, float redeyeratio)
    {
        try
        {
            string path = EnmyTypeConstans.GetZombiWrapperScenePath(name);
            var packed = GD.Load<PackedScene>(path);
            if (packed == null) return null;
            var instance = packed.Instantiate<Node2D>();
            if (tileIndex < 1 || tileIndex > YPointList.Count) return instance;
            var tilePos = YPointList[tileIndex - 1];
            var pos = tilePos;
            return Doing(instance, pos, name, redeyeratio);
        }
        catch (Exception ex) { } return null;
    }

    float HorizonYEnmyGen = GameContants.HorizonYEnmyGen;
    float HorizonYEnmyGenEnd = GameContants.HorizonYEnmyGenEnd;
    public Node2D GenerateEnemyOfPos(Vector2 pos, string name, float redeyeratio)
    {
        string path = EnmyTypeConstans.GetZombiWrapperScenePath(name);
        var packed = GD.Load<PackedScene>(path);
        if (packed == null) return null;
        return Doing(packed.Instantiate<Node2D>(), pos, name, redeyeratio);
    }

    Node2D Doing(Node2D instance, Vector2 pos, string name, float redeyeratio)
    {
        // 
        instance.Position = pos;
        // 设置缩放
        if (instance is IEnmy enmy)
        {
            enmy.SetObjName(name);
            enmy.SetInitScale(InitMoveSpeedScale, InitBeHurtScale, InitViewScale, InitAttackSpeedScale);
        }
        if (instance is IInit init)
        {
            init.Init(name);
        }
        //
        AddChild(instance);
        //
        if (instance is IEnmy enmy2)
        {
            enmy2.JudgeOpenRedEyeMode(redeyeratio);
        }
        // 加计数
        GameStatistic.Instance?.AddZombie(1);
        return instance;
    }
    public void SetGeneratorPosition(Vector2 pos)
    {
        this.Position = pos;
    }
}
