using Godot;
using System;
using ZVB4.Conf;

public partial class GuanZiCenterSystem : Node2D
{
    

    public static GuanZiCenterSystem Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void GenerateGuanZi(int geziIndex, int viewCode, float musenmyratio, Godot.Collections.Array plansList, Godot.Collections.Array enemyList)
    {
        Vector2 pos = GeZiWrapper.Instance.GetGeziByIndex(geziIndex - 1).Position;
        var scene = GD.Load<PackedScene>(FolderConstants.WaveThings + "guan_zi.tscn");
        var instance = scene.Instantiate<ThingsGuanZi>();
        ThingsGuanZi gz = instance;
        if (gz != null)
        {
            // 初始化
            gz.Init(pos, viewCode, musenmyratio, plansList, enemyList);
            var gcs = Instance;
            gcs?.AddChild(instance);
            //
            GameStatistic.Instance?.AddGuanZiChapterTotal(1);
        }
    }
}
