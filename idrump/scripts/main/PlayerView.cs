using Godot;
using System;

public partial class PlayerView : Node2D
{
    private AnimatedSprite2D animSprite;
    // 默认向右
    public void FlipByMove(float moveX)
    {
        if (moveX == 0) return;
        var scale = Scale;
        scale.X = moveX > 0 ? Math.Abs(scale.X) : -Math.Abs(scale.X);
        Scale = scale;
    }
    
    // 生成10个player_fx_star.tscn特效，每隔0.1s生成一个
    private int fxStarCount = 0;
    private const int fxStarTotal = 10;
    private float fxStarTimer = 0f;
    private PackedScene fxScene = null;
    private bool isSpawningFx = false;

    public void SpawnFxStars()
    {
        fxScene = GD.Load<PackedScene>("res://element/fxs/player_fx_star.tscn");
        fxStarCount = 0;
        fxStarTimer = 0f;
        isSpawningFx = true;
    }

    float starSpeed = 0.1f;

    public void FxStar(double delta)
    {
        if (isSpawningFx)
        {
            fxStarTimer += (float)delta;
            if (fxStarTimer >= starSpeed && fxStarCount < fxStarTotal)
            {
                fxStarTimer = 0f;
                var fx = fxScene.Instantiate<Node2D>();
                // 设置 fx 的全局位置为 PlayerView 的全局位置
                fx.GlobalPosition = GlobalPosition;
                // 将 fx 加入到主场景根节点
                GetTree().Root.AddChild(fx);
                fxStarCount++;
                if (fxStarCount >= fxStarTotal)
                {
                    isSpawningFx = false;
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        FxStar(delta);
    }

    public override void _Ready()
    {
        animSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        if (animSprite != null)
        {
            animSprite.Play("run");
        }
    }
}
