using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class GeZiWrapper : Node2D


{
	public static GeZiWrapper Instance { get; private set; }

    [Export]
    public int XGeziNum = 7;
    [Export]
    public int YGeziNum = 7;

    float XSize = 130;
    float YSize = 120;

    public override void _Ready()
    {
        Instance = this;
        geziPosList = GenerateAllGeziPositions();
        geziPosList = GetSortedGeziPositions();
        GenerateAllGeziNodes();
    }

    public IObj workingObj;

    List<Vector2> geziPosList = new List<Vector2>();

    // 设置当前工作对象
    public void SetWorkingObj(IObj obj)
    {
        workingObj = obj;
    }

    List<Node2D> geziNodeList = new List<Node2D>();

    public Node2D GetGeziByIndex(int index)
    {
        if (index < 0 || index >= geziNodeList.Count) {
            return null;
        }
        return geziNodeList[index];
    }

    /// <summary>
    /// 根据geziPosList批量生成Gezi节点并设置位置
    /// </summary>
    public void GenerateAllGeziNodes()
    {
        string scenePath = FolderConstants.WaveGround + "ge_zi.tscn";
        var geziScene = GD.Load<PackedScene>(scenePath);
        if (geziScene == null)
        {
            GD.PrintErr($"无法加载格子场景: {scenePath}");
            return;
        }
        foreach (var pos in geziPosList)
        {
            var gezi = geziScene.Instantiate<Node2D>();
            gezi.Position = pos;
            AddChild(gezi);
            geziNodeList.Add(gezi);
        }
    }

    /// <summary>
    /// 生成所有格子的中心坐标（以自身为中心，格子向四周铺开）
    /// </summary>
    public List<Vector2> GenerateAllGeziPositions()
    {
        var positions = new List<Vector2>();
        float x0 = GlobalPosition.X;
        float y0 = GlobalPosition.Y;

        float totalW = XGeziNum * XSize;
        float totalH = YGeziNum * YSize;

        float xStart, yStart;
        // X方向
        if (XGeziNum % 2 == 1)
            xStart = x0 - (XGeziNum / 2) * XSize;
        else
            xStart = x0 - (XGeziNum / 2 - 0.5f) * XSize;
        // Y方向
        if (YGeziNum % 2 == 1)
            yStart = y0 - (YGeziNum / 2) * YSize;
        else
            yStart = y0 - (YGeziNum / 2 - 0.5f) * YSize;

        for (int y = 0; y < YGeziNum; y++)
        {
            for (int x = 0; x < XGeziNum; x++)
            {
                float px = xStart + x * XSize;
                float py = yStart + y * YSize;
                positions.Add(new Vector2(px, py));
            }
        }
        return positions;
    }
    
    /// <summary>
    /// 返回按x从左到右，y从上到下排序的新坐标列表（左上角最小，右下角最大）
    /// </summary>
    public List<Vector2> GetSortedGeziPositions()
    {
        var sorted = new List<Vector2>(geziPosList);
        sorted.Sort((a, b) =>
        {
            if (Mathf.Abs(a.Y - b.Y) > 1e-3)
                return a.Y.CompareTo(b.Y); // y小的在上面
            return a.X.CompareTo(b.X); // x小的在左边
        });
        return sorted;
    }
}
