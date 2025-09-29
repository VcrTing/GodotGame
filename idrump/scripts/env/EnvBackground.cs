using Godot;
using System;

public partial class EnvBackground : Node2D
{
    string __BG_NAME = "BG";

    // 缩放背景图以适应屏幕
    public void ScaleBg()
    {
        // 获取屏幕大小
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        // 获取子节点中的Sprite2D
        Sprite2D sprite = GetNode<Sprite2D>(__BG_NAME);
        if (sprite != null && sprite.Texture != null)
        {
            // 计算缩放比例，短边填满屏幕，长边可超出
            Vector2 textureSize = sprite.Texture.GetSize();
            float scaleRatio = Math.Max(screenSize.X / textureSize.X, screenSize.Y / textureSize.Y);
            Vector2 scale = new Vector2(scaleRatio, scaleRatio);
            sprite.Scale = scale;
            GD.Print($"Scale: {scale}");
        }
    }

    public override void _Ready()
    {
        ScaleBg();
        // 设置位置为摄像机中心
        Camera2D camera = GetViewport().GetCamera2D();
        if (camera != null)
        {
            GD.Print($"Camera2D GlobalPosition: {camera.GlobalPosition}");
            Position = camera.GlobalPosition;
        }
    }
}
