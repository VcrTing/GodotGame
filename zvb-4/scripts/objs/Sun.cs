
using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class Sun : Node2D, IWorking, IReword
{
    int value = 50;
    AnimatedSprite2D view;

    private bool IsWorkingMode = false;
    private Vector2 targetWorldPos = Vector2.Zero;
    private float minScale = 0.7f; // 最小缩放比例
    private float moveSpeedMin = 400f; // 起始速度
    private float moveSpeedMax = 900f; // 最大速度
    private float movePhaseTime = 0.5f; // 加速到最大速度所需时间
    private float moveTimer = 0f;
    private Action onArriveTarget = null;

    float originScale = 1f;

    public void SetWorkingMode(bool working)
    {
        IsWorkingMode = working;
        if (working)
        {
            try
            {
                targetWorldPos = SunCenterSystem.Instance.GetLabelPosition();
            }
            catch
            {
                targetWorldPos = new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH);
            }
            moveTimer = 0f;
        }
    }

    public bool Init(string name, int number, int _value)
    {
        value = _value;
        float rd = (float)GD.RandRange(0f, 0.1f);
        originScale = 1.03f - rd;
        if (value < SunMoneyConstants.SunNormal) originScale = 0.66f - rd;
        if (value == SunMoneyConstants.SunNormal2) originScale = 1.13f - rd;
        if (value > SunMoneyConstants.SunNormal2) originScale = 1.43f - rd;
        view = GodotTool.GetViewAndAutoPlay(this);
        SetWorkingMode(true);
        return true;
    }

    void ReleaseSun()
    {
        try
        {
            AddSun();
            // 播放音效
            SoundFxController.Instance.PlayFx("Ux/coll", "coll_sun", 4);
        }
        catch
        {

        }
    }
    bool hasAdded = false;
    void AddSun()
    {
        if (hasAdded) return;
        hasAdded = true;
        // 设置 view 的透明度 = 0.618f
        if (view != null)
        {
            var c = view.Modulate;
            view.Modulate = new Color(c.R, c.G, c.B, 0.618f);
        }
        // 增加阳光方法
        if (SunCenterSystem.Instance == null) return;
        SunCenterSystem.Instance.AddValue(value);
        value = 0;
    }

    // 注册到达目标点的回调
    public void SetOnArriveTarget(Action callback)
    {
        onArriveTarget = callback;
    }

    // 可扩展接口，外部可指定目标点
    public void SetTargetWorldPos(Vector2 pos)
    {
        targetWorldPos = pos;
    }

    public override void _Ready()
    {
        rotating = GameTool.RandomRotatingV(rotating);
    }

    float rotating = 60f;
    public override void _Process(double delta)
    {
        if (IsWorkingMode)
        {
            moveTimer += (float)delta;
            if (moveTimer > 0.5f)
            {
                AddSun();
            }
            // 0~movePhaseTime内平滑加速，之后恒定最大速度
            float t = Mathf.Clamp(moveTimer / movePhaseTime, 0f, 1f);
            float speed = Mathf.Lerp(moveSpeedMin, moveSpeedMax, t);
            Vector2 curPos = GlobalPosition;
            Vector2 dir = (targetWorldPos - curPos);
            float dist = dir.Length();
            if (dist > 2f)
            {
                // 移动
                Vector2 move = dir.Normalized() * (float)delta * speed;
                if (move.Length() > dist) move = dir;
                GlobalPosition += move;
                // 根据距离缩放，距离越近scale越小，最小0.3
                float scaleVal = Mathf.Max(minScale, dist / 400f); // 400像素外为1，越近越小
                if (scaleVal > originScale) scaleVal = originScale;
                Scale = new Vector2(scaleVal, scaleVal);
            }
            else
            {
                // 到达目标，缩放到最小
                Scale = new Vector2(minScale, minScale);
                IsWorkingMode = false;
                onArriveTarget?.Invoke();
                onArriveTarget = null;
                // 开始淡出销毁流程
                if (!_isFadingOut)
                {
                    // 阳光到达
                    ReleaseSun();
                    _isFadingOut = true;
                    _fadeTime = 0f;
                }
            }
            // 轻微旋转自己
            Rotation += Mathf.DegToRad(rotating) * (float)delta;
        }
        
        // 淡出销毁逻辑
        if (_isFadingOut)
        {
            _fadeTime += (float)delta;
            float fadeDur = 0.2f;
            float alpha = Mathf.Lerp(1f, 0f, _fadeTime / fadeDur);
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
            if (_fadeTime >= fadeDur)
            {
                QueueFree();
            }
        }
    }

    public bool IsWorking() => IsWorkingMode;

    // --- 淡出销毁相关字段 ---

    private bool _isFadingOut = false;
    private float _fadeTime = 0f;
}
