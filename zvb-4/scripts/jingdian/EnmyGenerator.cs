
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
        var intervals = GetTenIntervals();
        tiles.Clear();
        tiles.AddRange(intervals);
        // GenerateEnemyByCode("zombi_s", 5);
    }
    [Export]
    public float TileMapW = GameContants.ZombieTileW; // 每个格子的宽度 
    [Export]
    public int TileNumHalf = GameContants.GrassNumHalf;

    List<Vector2> tiles = new List<Vector2>();
    /// <summary>
    /// 获取以本节点x轴为中心的10份区间坐标点（共10个，左5右5）
    /// </summary>
    /// <returns>长度为10的Vector2数组</returns>
    /// 
    /// 
    public Vector2[] GetTenIntervals()
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

    async void Generate(string type, int grid, bool israndom, float lazyme, int randomXRate)
    {
        if (lazyme > 0)
        {
            await ToSignal(GetTree().CreateTimer(lazyme), "timeout");
        }
        if (israndom)
        {
            Instance.GenerateEnemyByCode(type, randomXRate);
        }
        else
        {
            Instance.GenerateEnemyByCode(type, grid, randomXRate);
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
    public static void GenerateEnemiesByConfig(Godot.Collections.Array types, Godot.Collections.Array generator, string typesmode, string generatormode, float lazyme, int randomXRate)
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
                gen.Generate(types[i].AsString(), (int)generator[i], generatormode == "random", lazyme * i, randomXRate);
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
                    gen.Generate(types[idx].AsString(), (int)generator[idx], generatormode == "random", lazyme * i, randomXRate);
                }
            }
        }
    }

    /// <summary>
    /// 根据敌人代号名称生成敌人实例，随机格子编号
    /// </summary>
    /// <param name="code">敌人代号名称</param>
    /// <returns>生成的敌人节点（Node2D），失败返回null</returns>
    public Node2D GenerateEnemyByCode(string code, int randomXRate)
    {
        int max = TileNumHalf * 2;
        int tileIndex = (int)GD.RandRange(1, max);
        return GenerateEnemyByCode(code, tileIndex, randomXRate);
    }
    float DoRandomX(Vector2 tilePos, int randomXRate)
    {
        if (randomXRate <= 0)
        {
            return tilePos.X;
        }
        float left = tilePos.X - TileMapW / 2f;
        float right = tilePos.X + TileMapW / 2f;
        float randX = (float)GD.RandRange(left, right);
        return randX;
    }

    /// <summary>
    /// 根据敌人代号名称和格子编号生成敌人实例，x坐标在格子区间内随机
    /// </summary>
    /// <param name="code">敌人代号名称</param>
    /// <param name="tileIndex">格子编号（1-10）</param>
    /// <returns>生成的敌人节点（Node2D），失败返回null</returns>
    public Node2D GenerateEnemyByCode(string name, int tileIndex, int randomXRate)
    {
        string path = EnmyTypeConstans.GetZombiWrapperScenePath(name);
        var packed = GD.Load<PackedScene>(path);
        if (packed == null)
        {
            GD.PrintErr($"未找到敌人场景: {path}");
            return null;
        }
        //
        var instance = packed.Instantiate<Node2D>();
        // 取格子区间
        if (tileIndex < 1 || tileIndex > tiles.Count)
        {
            GD.PrintErr($"格子编号超出范围: {tileIndex}");
            // AddChild(instance);
            return instance;
        }
        var tilePos = tiles[tileIndex - 1];
        var pos = instance.Position;
        pos.X = DoRandomX(tilePos, randomXRate);
        //
        pos.Y = pos.Y + genY;
        return Doing(instance, pos, name);
    }

    float genY = GameContants.HorizonYEnmyGen;
    public Node2D GenerateEnemyOfPos(Vector2 pos, string name)
    {
        string path = EnmyTypeConstans.GetZombiWrapperScenePath(name);
        var packed = GD.Load<PackedScene>(path);
        if (packed == null) return null;
        return Doing(packed.Instantiate<Node2D>(), pos, name);
    }

    Node2D Doing(Node2D instance, Vector2 pos, string name)
    {
        // 
        instance.Position = pos;
        // 设置缩放
        if (instance is IEnmy enmy)
        {
            enmy.SetObjName(name);
            enmy.SetInitScale(InitMoveSpeedScale, InitBeHurtScale, InitViewScale, InitAttackSpeedScale);
        }
        if (instance is IInit init) {
            init.Init(name);
        }
        //
        AddChild(instance);
        // 加计数
        GameStatistic.Instance?.AddZombie(1);
        return instance;
    }
    public void SetGeneratorPosition(Vector2 pos)
    {
        this.Position = pos;
    }
}
