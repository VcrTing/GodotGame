
using Godot;
using ZVB4.Conf;

public static class PlayerTool
{
    public static string GetShooterWorkTableScenePath(EnumPlayerShooterMode mode)
    {
        switch (mode)
        {
            case EnumPlayerShooterMode.TouchShooter:
                return FolderConstants.WavePlayer + "working/shooter_work_table.tscn";
            case EnumPlayerShooterMode.LineShooter:
                return FolderConstants.WavePlayer + "working/shooter_work_line.tscn";
            default:
                return "";
        }
    }
    public static Node2D GenerateWorkTable(EnumPlayerShooterMode mode, Node parent)
    {
        string scenePath = GetShooterWorkTableScenePath(mode);
        if (scenePath == "") return null;
        var scene = GD.Load<PackedScene>(scenePath);
        if (scene == null) return null;
        var instance = scene.Instantiate<Node2D>();
        parent.AddChild(instance);
        return instance;
    }

    public static int ComputedShooterShootRatio(int allowAttackNum)
    {
        //
        if (PlayerController.Instance != null)
        {
            float ratio = PlayerController.Instance.GetShootRatio();
            if (ratio < 0f) ratio = 0f;
            if (ratio > 999)
            {
                allowAttackNum = -1;
            }
            else
            {
                allowAttackNum = (int)(allowAttackNum * ratio);
            }
        }
        return allowAttackNum;
    }
}