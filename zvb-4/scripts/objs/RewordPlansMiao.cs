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
        // 存活10秒后自动销毁
        aliveTimer += (float)delta;
        if (aliveTimer >= aliveDuration)
        {
            QueueFree();
        }
        
        if (!init) return;


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
    float workingTime = 1.5f;
    bool isLock = false;
    float h = GameContants.ScreenHalfH - 90;
    async void StartToFlowerPeng(FlowerPeng flowerPeng)
    {
        if (isLock) return;
        isLock = true;
        SetWorkingMode(true);
        // Vector2 pos = flowerPeng.GlobalPosition;
        // pos = flowerPeng.ToGlobal(pos);
        RefreshAlivePosition(new Vector2(0, h));
        flowerPeng.DelayGeneratePlansMiao(workingTime, PlansName);
        await this.ToSignal(this.GetTree().CreateTimer(workingTime + 0.2f), "timeout");
        QueueFree();
    }

    float checkTimer = 0f;
    float checkInterval = 0.3f;

    float aliveTimer = 0f;
    float aliveDuration = 5f;

    public override void _Ready()
    {
        alivePosition = Position;
        InitPosition = Position;
        int a = (int)GD.RandRange(0, 20);
        aliveDuration += (a / 10f);
        // Init();
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

    bool init = false;
    public void Init()
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
        init = true;
    }

    public void Init(Vector2 position, string plansName)
    {
        Position = position;
        InitPosition = position;
        PlansName = plansName;
        init = true;
    }

    async void Test()
    {
        // 延迟1秒后开始工作
        await this.ToSignal(this.GetTree().CreateTimer(1f), "timeout");
        RefreshAlivePosition(new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH - 100));
        SetWorkingMode(true);
    }
}
