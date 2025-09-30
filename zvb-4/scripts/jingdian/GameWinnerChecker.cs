using Godot;
using System;

public partial class GameWinnerChecker : Node2D
{
    public static GameWinnerChecker Instance { get; private set; }
    // 时间点数组
    private System.Collections.Generic.List<float> timePoints = new();
    private float minTimePoint = float.MaxValue;
    private float timer = 0f;

    public override void _Ready()
    {
        Instance = this;
        // 示例：添加一些时间点
    }

    public void AddTimePoint(float timePoint)
    {
        timePoints.Add(timePoint);
        UpdateMinTimePoint();
    }

    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 10f;

    float workingSnapTime = 0f;

    public override void _Process(double delta)
    {
        timer += (float)delta;
        if (!isWaiting && minTimePoint != float.MaxValue && timer >= minTimePoint)
        {
            isWaiting = true;
            waitTimer = 0f;
            workingSnapTime = 0f;
        }
        if (isWaiting)
        {
            waitTimer += (float)delta;
            if (waitTimer >= waitDuration)
            {
                isWaiting = false;
                UpdateMinTimePoint();
            }
            workingSnapTime += (float)delta;
            if (workingSnapTime >= 0.5f)
            {
                workingSnapTime = 0f;
                OnTimePointReached(minTimePoint);
            }
        }

    }

    // 获取并删除最小时间点
    private void UpdateMinTimePoint()
    {
        if (timePoints.Count == 0)
        {
            minTimePoint = float.MaxValue;
            return;
        }
        minTimePoint = float.MaxValue;
        int minIdx = -1;
        for (int i = 0; i < timePoints.Count; i++)
        {
            if (timePoints[i] < minTimePoint)
            {
                minTimePoint = timePoints[i];
                minIdx = i;
            }
        }
        if (minIdx >= 0)
        {
            timePoints.RemoveAt(minIdx);
        }
    }

    bool hasChecked = false;

    // 到达时间点时执行的方法
    private void OnTimePointReached(float timePoint)
    {
        // 等待3秒后再执行下一个时间点
        bool isAllDie = GameStatistic.Instance?.IsAllZombieDead() ?? false;
        if (isAllDie)
        {
            if (hasChecked) return;
            GD.Print("所有僵尸已被消灭，玩家获胜！");
            // 在这里执行玩家获胜的逻辑
            CapterWinPopup.Instance?.ShowPopup();
            hasChecked = true;
        }
        else
        {
            GD.Print("仍有僵尸存活，继续游戏。");
            // 继续游戏逻辑
        }
    }
}
