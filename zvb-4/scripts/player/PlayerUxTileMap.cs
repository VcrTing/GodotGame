
using System.Collections.Generic;
using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class PlayerUxTileMap : TileMapLayer, IPlansPlanting
{
    // 单元格占用列表
    private HashSet<Vector2I> _usedCells = new HashSet<Vector2I>();
    // 判断指定pos所在单元格是否被占用
    public bool IsCellBeUsed(Vector2 pos)
    {
        if (TileSet == null) return false;
        var cell = LocalToMap(ToLocal(pos));
        return _usedCells.Contains(cell);
    }
    // 标记单元格为占用
    public void UseCell(Vector2 pos)
    {
        var cell = LocalToMap(ToLocal(pos));
        _usedCells.Add(cell);
    }
    // 释放单元格占用
    public void ReleaseCell(Vector2 pos)
    {
        var cell = LocalToMap(ToLocal(pos));
        _usedCells.Remove(cell);
    }

    // 获取格子中心点（返回本节点坐标系下的中心点，自动考虑父节点位移）
    Vector2 GetCellCenter(Vector2 pos)
    {
        if (TileSet == null) return pos;
        var cell = LocalToMap(ToLocal(pos));
        var cellSize = TileSet.TileSize;
        var localCenter = MapToLocal(cell) + cellSize / 2;
        // 如果父节点有位移，MapToLocal已自动考虑父节点变换，localCenter就是本节点坐标系下的中心点
        return localCenter;
    }
    // 为指定位置
    // 是否能种植
    public bool CanZhongZhiPlans(Vector2 pos, string planName)
    {
        // 特殊植物判断，planName

        // 通常情况下，只要单元格未被占用，就可以种植
        if (IsCellBeUsed(pos))
        {
            GD.Print("该位置已被占用，无法种植");
            return false;
        }
        return true;
    }
    // 种植物
    public bool ZhongZhiPlans(Vector2 pos, string planName)
    {
        // 判断
        bool canZhiZhi = CanZhongZhiPlans(pos, planName);
        if (!canZhiZhi) return false;
        // 开始种植
        string scenePath = PlansConstants.GetPlanScene(planName);
        if (!string.IsNullOrEmpty(scenePath))
        {
            var scene = GD.Load<PackedScene>(scenePath);
            if (scene != null)
            {
                Node2D pls = (Node2D)scene.Instantiate();
                // 拿到格子中心点
                AddChild(pls);
                Vector2 cellCenter = GetCellCenter(pos);
                pls.Position = cellCenter;
                // 调整视图
                // if (pls is IObj iobj) iobj.AdjustView();
                    
                // 设置工作模式
                if (pls is IWorking working)
                {
                    //
                    SoundFxController.Instance?.PlayFx("Ux/zhongxia", "ZhongXia", 4, pls.GlobalPosition);
                    working.SetWorkingMode(true);
                    // 标记占用
                    if (PlansConstants.IsWillZhanYongGeZi(planName))
                    {
                        UseCell(pos);
                    }
                    // 成功
                    return true;
                }
            }
        }
        return false;
    }
}
