using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class Money : RigidBody2D, IWorking, IReword
{
    int value = 50;
    string name = "";
    AnimatedSprite2D view;

    private bool IsWorkingMode = false;
    private Vector2 targetWorldPos = Vector2.Zero;
    private float minScale = 0.5f; // 最小缩放比例
    private float moveSpeedMin = 300f; // 起始速度
    private float moveSpeedMax = 700f; // 最大速度
    private float movePhaseTime = 0.5f; // 加速到最大速度所需时间
    private float moveTimer = 0f;
    private Action onArriveTarget = null;

    float originScale = 1f;

    public void SetWorkingMode(bool working)
    {
        IsWorkingMode = working;
        if (working)
        {
            // 默认目标为左上角世界坐标
            try
            {
                targetWorldPos = SunCenterSystem.Instance.GetLabelPosition();
                // GD.Print("Sun Target Pos: " + targetWorldPos);
            }
            catch
            {
                targetWorldPos = new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH);
            }
            moveTimer = 0f;
            GravityScale = 0f;
        }
    }
    void TanShe()
    {
        // 随机方向弹射
        float angle = (float)GD.RandRange(0, Mathf.Pi * 2);
        float impulse = (float)GD.RandRange(280f, 460f); // 小尺度弹射
        Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        ApplyImpulse(Vector2.Zero, dir * impulse);
    }

    float speed = 340f;
    public async void Init()
    {
        int rd = (int)GD.RandRange(0, 50);
        float v = 1.2f - (rd / 100f);
        originScale = v;
        Scale = new Vector2(originScale, originScale);
        // GD.Print("原始缩放 =" + v);
        speed += ((float)rd * 2);
        GravityScale = GravityScale + (rd / 100f);
        // LLL
        TanShe();

        string n = SunMoneyConstants.GetRnadomMoneyName();
        value = SunMoneyConstants.GetMoneyValue(n);
        await ToSignal(GetTree().CreateTimer(1.5f + (rd / 200f)), "timeout");
        init = true;
        Init(n, 1, value);
    }
    public bool Init(string name, int number, int _value)
    {
        value = _value;
        view = GodotTool.GetViewAndAutoPlay(this);
        SetWorkingMode(true);
        SetOnArriveTarget(() =>
        {
            ReleaseVlaue();
            QueueFree();
        });
        ReleaseVlaue();
        return true;
    }

    void ReleaseVlaue()
    {
        try
        {
            if (hasAdded) return;
            GD.Print("Release Sun " + value);
            // 播放音效
            SoundFxController.Instance.PlayFx("Ux/coll", "coll_sun", 4);
            hasAdded = true;
            // 增加阳光方法
            if (MoneyCenterSystem.Instance == null) return;
            MoneyCenterSystem.Instance.AddMoney(value);
            value = 0;
        }
        catch
        {

        }
    }
    bool hasAdded = false;

    // 注册到达目标点的回调
    public void SetOnArriveTarget(Action callback)
    {
        onArriveTarget = callback;
    }

    public override void _Ready()
    {
        GravityScale = SunMoneyConstants.GetMoneyGravity();
        CallDeferred(nameof(TanShe)); // 延迟到物理帧
        Init();
    }

    bool init = false;

    public override void _Process(double delta)
    {
        if (init)
        {
            if (IsWorkingMode)
            {
                // KKK
                Vector2 curPos = GlobalPosition;
                Vector2 dir = targetWorldPos - curPos;
                float dist = dir.Length();
                float moveStep = speed * (float)delta;
                if (dist > moveStep)
                {
                    GlobalPosition += dir.Normalized() * moveStep;
                }
                else
                {
                    GlobalPosition = targetWorldPos;
                    IsWorkingMode = false;
                    onArriveTarget?.Invoke();
                    onArriveTarget = null;
                }
            }
        }
    }
}
