using Godot;
using System;

public partial class PlayerFxStar : Node2D
{
	private float timer = 0f;
	private enum State { FadeIn, Hold, FadeOut, Done }
	private State state = State.FadeIn;

    [Export]
    public float defaultScale = 2.8f;

	public override void _Ready()
    {
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
        // 随机缩放，变化不大（0.85~1.15倍）
        float scale = (float)GD.RandRange(0, 1.3);
        Scale = new Vector2(defaultScale + scale, defaultScale + scale);
        state = State.FadeIn;
        timer = 0f;
    }
    
	public override void _Process(double delta)
    {
        timer += (float)delta;

        // 总生命周期 0.2 + 0.8 + 0.2 = 1.2 秒
        float totalLife = 1.2f;
        float shrinkStart = 0.0f;
        float shrinkEnd = totalLife;
        float shrinkT = Mathf.Clamp((timer +
            (state == State.FadeIn ? 0f : (state == State.Hold ? 0.2f : 1.0f))) / totalLife, 0f, 1f);
        // 0.0~1.0 之间插值，scale 从初始缩放到 0.2 倍
        float scaleLerp = Mathf.Lerp(1f, 0.2f, shrinkT);
        Scale = new Vector2(defaultScale * scaleLerp, defaultScale * scaleLerp);

        if (state == State.FadeIn)
        {
            if (timer >= 0.2f)
            {
                Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 1f);
                state = State.Hold;
                timer = 0f;
            }
        }
        else if (state == State.Hold)
        {
            if (timer >= 0.8f)
            {
                state = State.FadeOut;
                timer = 0f;
            }
        }
        else if (state == State.FadeOut)
        {
            float t = Mathf.Clamp(timer / 0.2f, 0f, 1f);
            float alpha = 1f - t;
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, alpha);
            if (timer >= 0.2f)
            {
                state = State.Done;
                QueueFree();
            }
        }
    }
}
