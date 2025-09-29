using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Entity;

public partial class FlowerPengSystem : Node2D
{
    float locationY = 0;

    float pingMuBianJu = 90;
    float pengW = 120;
    float ouShuUpY = 120;
    int flowerPengNumHalf = GameContants.FlowerPengNumHalf;
    public System.Collections.Generic.List<Vector2> PositionList { get; private set; } = new System.Collections.Generic.List<Vector2>();

    public override void _Ready()
    {
        GenerateGeziPosition();

        // 初始化就生成多少个花盆
        Init();
    }

    void GenerateGeziPosition()
    {
        // 左边（负数坐标）
        for (int i = 0; i < flowerPengNumHalf; i++)
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
        for (int i = 0; i < flowerPengNumHalf; i++)
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
    }

    EntityPlayerData playerData;

    async void Init()
    {
        var sdm = SaveDataManager.Instance;
        if (sdm != null)
        {
            playerData = sdm.GetPlayerData();
            int numNow = playerData.CapterFlowerPengNumNow;
            __Init(8);
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
        var flowerPengScene = GD.Load<PackedScene>(FolderConstants.WaveUx + "flower_peng.tscn");
        for (int i = 0; i < limit; i++)
        {
            var flowerPeng = flowerPengScene.Instantiate<Node2D>();
            flowerPeng.Position = PositionList[i];
            AddChild(flowerPeng);
        }
    }
}
