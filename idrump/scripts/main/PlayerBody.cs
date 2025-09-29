using Godot;
using System;

public partial class PlayerBody : CharacterBody2D
{

    [Export]
    public float Speed = 200f;
    // 操控
    [Export]
    public float Gravity = 800f;


    [Export]
    // X轴最大加速度
    public float MaxAcceleration = 1000f;
    [Export]
    // X轴最小加速度
    public float MinAcceleration = 100f;
    // 当前帧加速度
    private float currentAcceleration = 0f;
    [Export]
    // X轴减速
    public float Deceleration = 1200f;


    // 记录初始位置
    private Vector2 startPosition;

    public void ResetPosition()
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
    }

    //
    PlayerView playerView;
    PlayerSoundCenter playerSoundCenter;

    public override void _Ready()
    {
        // 记录初始位置
        startPosition = Position;
        // 获取PlayerView子节点
        playerView = GetNodeOrNull<PlayerView>("PlayerView");
        playerSoundCenter = GetNodeOrNull<PlayerSoundCenter>("PlayerSoundCenter");
    }

    public override void _Process(double delta)
    {
        Control(delta);
    }

    [Export]
    public float JumpVelocity = -400f;
    int jumpCount = 0;

    public void Control(double delta)
    {
        // X
        float moveX = 0f;
        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
        {
            moveX -= 1f;
        }
        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
        {
            moveX += 1f;
        }

        // Y
        float y = Velocity.Y;
        float dt = (float)delta;
        y += Gravity * dt;

        // 跳跃
        if ((Input.IsActionJustPressed("ui_accept") || Input.IsKeyPressed(Key.Space)))
        {
            if (jumpCount <= 0)
            {
                y = JumpVelocity;
                jumpCount++;
            }
        }
        if (IsOnFloor())
        {
            jumpCount = 0;
        }

        // 检测上升/下降区间
        if (y < 0 && prevYVelocity >= 0)
        {
            WhenUpStart();
        }
        else if (y >= 0 && prevYVelocity < 0)
        {
            WhenUpEnd();
        }
        //
        if (y > 0 && prevYVelocity <= 0)
        {
            WhenDownStart();
        }
        // 只在Y轴从下落到静止时执行一次WhenDownEnd
        if (prevYVelocity > 0 && Mathf.Abs(y) < 0.1f)
        {
            WhenDownEnd();
        }
        prevYVelocity = y;

        // 移动
        Move(moveX, y, dt);
    }

    // 上一帧Y轴速度
    private float prevYVelocity = 0f;

    public void Move(float moveX, float vY, float delta)
    {
        Vector2 velocity = Velocity;
        // X轴加速/减速（曲线递减）
        if (moveX != 0f)
        {
            float target = moveX * Speed;
            float speedDiff = Mathf.Abs(target - velocity.X);
            float ratio = Mathf.Clamp(speedDiff / Speed, 0, 1);
            // 刚开始加速度大，越接近目标越小
            currentAcceleration = Mathf.Lerp(MinAcceleration, MaxAcceleration, ratio);
            velocity.X = Mathf.MoveToward(velocity.X, target, currentAcceleration * delta);
        }
        else
        {
            // 松开按键，恢复最大加速度
            currentAcceleration = MaxAcceleration;
            velocity.X = Mathf.MoveToward(velocity.X, 0, Deceleration * delta);
        }
        // Y轴速度
        velocity.Y = vY;

        // 应用速度
        Velocity = velocity;
        // 翻转角色
        playerView?.FlipByMove(moveX);
        // 移动
        MoveAndSlide();
    }

    public void WhenUpStart()
    {
        GD.Print("WhenUpStart");

        // Fx
        playerView?.SpawnFxStars();

        // 播放音频
        playerSoundCenter?.WhenPlayerDump();

    }
    public void WhenUpEnd()
    {
        GD.Print("WhenUpEnd");
    }
    public void WhenDownStart()
    {
        GD.Print("WhenDownStart");
    }
    public void WhenDownEnd()
    {
        GD.Print("WhenDownEnd");
    }
}
