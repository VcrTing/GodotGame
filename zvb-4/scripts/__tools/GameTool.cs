
using System;
using Godot;
using ZVB4.Conf;

public static class GameTool
{
    // 获取子弹的初始速度
    public static float GetBulletInitialSpeed(string bulletName)
    {
        float SpeedInit = BulletConstants.GetSpeed(bulletName);
        var rand = new Random();
        float factor = 1f + (float)rand.NextDouble() * 0.2f;
        float Speed = SpeedInit * factor;
        return Speed;
    }
    public static float FixBulletSpeedByY(float SpeedInit, Vector2 Position, float maxY)
    {
        float curY = Position.Y;
        if (curY < -maxY)
        {
            // 速度为负，表示子弹出界，死亡。
            return 0f;
        }
        else
        {
            // 可根据y值做线性降速（如需要）
            float t = Mathf.Clamp((curY + maxY) / maxY, 0f, 1f);
            return SpeedInit * t;
        }
    }
    public static bool RunnerBulletZeroWhenDieFx(Node2D node2D, CanvasItem view, double delta, ref float fadeElapsed, float fadeDuration = AnimationConstants.BulletFadeDieDuration, float fadeLowest = 0f)
    {
        if (view == null) return false;
        Vector2 Scale = node2D.Scale;
        if (Scale.X > 0.11f)
        {
            float scaleT = Mathf.Clamp(fadeElapsed / fadeDuration, 0f, 1f);
            float newScale = Mathf.Lerp(Scale.X, 0.1f, scaleT);
            node2D.Scale = new Vector2(newScale, newScale);
        }
        try
        {
            if (view != null)
            {
                if (fadeElapsed < fadeDuration)
                {
                    fadeElapsed += (float)delta;
                    float t = Mathf.Clamp(fadeElapsed / fadeDuration, 0f, 1f);
                    float alpha = Mathf.Lerp(1f, fadeLowest, t);
                    view.Modulate = new Color(1, 1, 1, alpha);
                }
                else
                {
                    view.Modulate = new Color(1, 1, 1, fadeLowest);
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"RunnerBulletZeroWhenDieFx Exception: {ex.Message}");
        }
        return true;
    }
    public static bool RunnerBulletZeroWhenDieFx(Node2D node2D, double delta, ref float fadeElapsed, float fadeDuration = AnimationConstants.BulletFadeDieDuration, float fadeLowest = 0f)
    {
        var view = node2D.GetNodeOrNull<CanvasItem>(NameConstants.View);
        return RunnerBulletZeroWhenDieFx(node2D, view, delta, ref fadeElapsed, fadeDuration, fadeLowest);
    }

    // 生成奖励组
    public static bool GenReword(string rName, int count, int v, Node2D parent)
    {
        string sc = RewordConstants.GetRewordGroupScene(rName);
        if (string.IsNullOrEmpty(sc)) return false;
        var groupSc = GD.Load<PackedScene>(sc);
        var group = groupSc.Instantiate<Node2D>();
        parent.AddChild(group);

        // CCC
        float offsetX = parent.Position.X < 0 ? -60f : 60f;
        Vector2 spawnPos = new Vector2(offsetX, 0);

        if (group is RewordGroup rg)
        {
            rg.SpawnReword(rName, count, spawnPos, v);
            return true;
        }
        return false;
    }

    public static Vector2 RotateVector2(Vector2 d, float degrees)
    {
        float rad = Mathf.DegToRad(degrees);
        Vector2 dir = d.Rotated(rad).Normalized();
        return dir;
    }
    public static Vector2 RotateVector22(Vector2 d, float degrees)
    {
        float rad = Mathf.DegToRad(degrees);
        float leftAngle = d.Angle() + rad;
        Vector2 leftDirection = new Vector2(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle));
        return leftDirection;
    }
    public static Vector2 OffsetPosition(Vector2 pos, float v)
    {
        return new Vector2(pos.X + v, pos.Y);
    }

    public static float RandomRotatingV(float rotating) {
        int i = GD.RandRange(0, 100);
        rotating += (i / 10);
        if (i < 50)
        {
            rotating = 0 - rotating;
        }
        return rotating;
    }
}