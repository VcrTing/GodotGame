using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class RewordPlansMiao : Node2D, IWorking
{
    Vector2 alivePosition;
    Vector2 InitPosition;
    string PlansName = "";

    public void RefreshAlivePosition(Vector2 position)
    {
        alivePosition = position;
    }

    float speed = 600f; // 可调整速度
    public override void _Process(double delta)
    {
        if (isWorking)
        {
            // 加速移动到alivePosition
            Vector2 toTarget = alivePosition - Position;
            if (toTarget.Length() > 1f)
            {
                Position += toTarget.Normalized() * speed * (float)delta;
            }
            else
            {
                Position = alivePosition;
            }
            
        }
        // 存活10秒后自动销毁
        aliveTimer += (float)delta;
        if (aliveTimer >= aliveDuration)
        {
            QueueFree();
        }

        // 自动追踪花盆
        checkTimer += (float)delta;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            if (!isLock)
            {
                var flowerPeng = FlowerPengSystem.Instance?.GetAUseFullFlowerPeng();
                if (flowerPeng != null)
                {
                    StartToFlowerPeng(flowerPeng);
                }
            }
        }
    }
    float workingTime = 1f;
    bool isLock = false;
    async void StartToFlowerPeng(FlowerPeng flowerPeng)
    {
        if (isLock) return;
        isLock = true;
        SetWorkingMode(true);
        RefreshAlivePosition(flowerPeng.GlobalPosition);
        flowerPeng.DelayGeneratePlansMiao(workingTime, PlansName);
        await this.ToSignal(this.GetTree().CreateTimer(workingTime + 0.2f), "timeout");
        QueueFree();
    }

    float checkTimer = 0f;
    float checkInterval = 0.5f;

    float aliveTimer = 0f;
    float aliveDuration = 4f;

    public override void _Ready()
    {
        alivePosition = Position;
        InitPosition = Position;
        Init();
    }
    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        // 拿到可用苗
        FxShiningLight fx = GetNodeOrNull<FxShiningLight>("FxShiningLight");
        if (fx != null)
        {
            if (isWorking)
            {
                fx.DieShining();
            }
            else
            {
                fx.ReShining();
            }
        }
    }

    void Init()
    {
        var rwm = RewordMiaoCenterSystem.Instance;
        if (rwm != null)
        {
            // PlansName = PlansConstants.GetRandomPlansName();
            PlansName = rwm.GetRandomPlansNameWithPowerWeight();
        }
        else
        {
            PlansName = PlansConstants.GetRandomPlansName();
            GD.Print("随机到的植物是: " + PlansName);
        }
        SoundFxController.Instance?.PlayFx("Ux/suprise", "suprise", 4, GlobalPosition);
    }

    async void Test()
    {
        // 延迟1秒后开始工作
        await this.ToSignal(this.GetTree().CreateTimer(1f), "timeout");
        RefreshAlivePosition(new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH - 100));
        SetWorkingMode(true);
    }
}
