using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Entity;

public partial class FlowerPengSystem : Node2D
{
    public static FlowerPengSystem Instance { get; private set; }
    float locationY = 0;

    float pingMuBianJu = 90;
    float pengW = 120;
    float ouShuUpY = 120;
    int flowerPengNumHalf = GameContants.FlowerPengNumHalf;
    public System.Collections.Generic.List<Vector2> PositionList { get; private set; } = new System.Collections.Generic.List<Vector2>();

    public override void _Ready()
    {
        Instance = this;
        GenerateGeziPosition();
        // 初始化就生成多少个花盆
        // Init();
    }

    void GenerateGeziPosition()
    {
        // 左边（负数坐标）
        for (int i = 0; i < 4; i++)
        {
            float x = -(pingMuBianJu + i * pengW); // 每份中心点
            float y = locationY;
            if (i % 2 == 0)
            {
            }
            else
            {
                y -= ouShuUpY;
            }
            PositionList.Add(new Vector2(x, y));
        }
        // 右边（正数坐标）
        for (int i = 0; i < 4; i++)
        {
            float x = pingMuBianJu + i * pengW; // 每份中心点
            float y = locationY;
            if (i % 2 == 0)
            {
            }
            else
            {
                y -= ouShuUpY;
            }
            PositionList.Add(new Vector2(x, y));
        }

        // 额外
        float extraY = locationY - (ouShuUpY * 2.6f);
        PositionList.Add(new Vector2(360, extraY));
        PositionList.Add(new Vector2(-360, extraY));
    }

    EntityPlayerData playerData;

    public async void Init(int i = -1)
    {
        var sdm = SaveDataManager.Instance;
        if (sdm != null)
        {
            playerData = sdm.GetPlayerData();
            int numNow = playerData.CapterFlowerPengNumNow;
            if (i != -1)
            {
                numNow = i;
            }
            // GD.Print("玩家现在多少花盆: " + numNow);
            __Init(numNow);
        }
        else
        {
            // 0.2秒后再执行一次Init
            await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
            Init();
        }
    }


    void __Init(int limit)
    {
        if (limit >= flowerPengNumHalf * 2)
        {
            limit = (flowerPengNumHalf * 2);
        }
        if (limit <= 0)
        {
            limit = 1;
        }
        if (limit > PositionList.Count)
        {
            limit = PositionList.Count;
        }
        // 实例化flower_peng
        var flowerPengScene = GD.Load<PackedScene>(FolderConstants.WaveGround + "flower_peng.tscn");
        for (int i = 0; i < limit; i++)
        {
            GenerateFlowerPengByCode(flowerPengScene, i);
        }
    }

    // 根据编号生成花盆
    void GenerateFlowerPengByCode(PackedScene flowerPengScene, int code)
    {
        var flowerPeng = flowerPengScene.Instantiate<Node2D>();
        flowerPeng.Position = PositionList[code];
        AddChild(flowerPeng);
        // 记录花盆
        flowerPengList.Add(flowerPeng as FlowerPeng);
    }

    List<FlowerPeng> flowerPengList = new List<FlowerPeng>();


    // 获取所有可用花盆
    public int GetNowFlowerPengNum()
    {
        return flowerPengList.Count;
    }
    public List<FlowerPeng> GetUseFullFlowerPeng()
    {
        List<FlowerPeng> result = new List<FlowerPeng>();
        foreach (var item in flowerPengList)
        {
            if (item.IsAllowPlans())
            {
                result.Add(item);
            }
        }
        return result;
    }
    public FlowerPeng GetAUseFullFlowerPeng()
    {
        try
        {
            List<FlowerPeng> src = GetUseFullFlowerPeng();
            if (src.Count == 0) return null;
            int idx = (int)GD.RandRange(0, src.Count);
            return src[idx];
        }
        catch (Exception ex)
        {
            GD.PrintErr("GetAUseFullFlowerPeng error: " + ex.Message);
            return null;
        }
    }
}
