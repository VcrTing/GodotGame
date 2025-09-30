using Godot;
using System;
using ZVB4.Conf;

public partial class FxShiningLight : Node2D
{
    private Node2D view;
    private Node2D viewExtra;

    public override void _Ready()
    {
        view = GetNodeOrNull<Node2D>(NameConstants.View);
        viewExtra = GetNodeOrNull<Node2D>(NameConstants.ViewExtra);
    }

    public override void _Process(double delta)
    {
        // View顺时针旋转
        if (view != null)
        {
            view.Rotation += Mathf.DegToRad(90f) * (float)delta; // 每秒180度
        }
        // ViewExtra逆时针慢速旋转
        if (viewExtra != null)
        {
            viewExtra.Rotation -= Mathf.DegToRad(30f) * (float)delta; // 每秒30度
        }

        if (isDied)
        {
            diedTimer += (float)delta;
            float t = Mathf.Clamp(diedTimer / diedDuration, 0f, 1f);
            float alpha = 1f - t;
            float scale = 1f - lowestScale * t;
            // 设置自身和子节点透明度和缩放
            SetAlphaAndScale(alpha, scale);
        }
    }

    bool isDied = false;
    float diedTimer = 0f;
    float lowestScale = 0.5f;
    float diedDuration = 1f;
    public void DieShining()
    {
        isDied = true;
        diedTimer = 0f;
    }
    public void ReShining()
    {
        isDied = false;
        diedTimer = 0f;
        // 恢复透明度和缩放
        SetAlphaAndScale(1f, 1f);
    }

    void SetAlphaAndScale(float alpha, float scale)
    {
        // 设置自身
        if (this is CanvasItem ci)
        {
            ci.Modulate = new Color(1, 1, 1, alpha);
        }
        if (this is Node2D n2d)
        {
            n2d.Scale = new Vector2(scale, scale);
        }
    }
}
