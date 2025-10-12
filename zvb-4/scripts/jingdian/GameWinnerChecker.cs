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
        minTimePoint = timePoint;
        GD.Print($"AddTimePoint: {timePoint}");
        // UpdateMinTimePoint();
    }

    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 10f;

    float workingSnapTime = 0f;

    public override void _Process(double delta)
    {
        timer += (float)delta;
        if (!isWaiting && timer >= minTimePoint)
        {
            isWaiting = true;
        }
        if (isWaiting)
        {
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
            minTimePoint = 100f;
            return;
        }
        minTimePoint = 100f;
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
    bool hasPrinted = false;

    int capnum = -1;

    // 到达时间点时执行的方法
    private void OnTimePointReached(float timePoint)
    {
        if (capnum == -1) {
            var ins = SaveGamerRunnerDataManger.Instance;
            capnum = ins.GetCapterNumber();
        }
        if (hasPrinted)
        {

        }
        else
        {
            hasPrinted = true;
        }

        // 等待3秒后再执行下一个时间点
        bool isWin = false;

        if (ChapterTool.IsJinDian(capnum))
        {
            isWin = GameStatistic.Instance?.JinDianWinCheck() ?? false;
        }
        else if (ChapterTool.IsGuanZi(capnum)) {
            isWin = GameStatistic.Instance?.GuanZiWinCheck() ?? false;
        }
        //
        if (isWin)
        {
            if (hasChecked) return;
            CapterWinPopup.Instance?.ShowPopup();
        }
    }
}
