


using System;
using Godot;
using ZVB4.Conf;

namespace ZVB4.Tool
{
    public static class ViewTool
    {

        public static float GetYouMinScale(float defaultScale)
        {
            return GameContants.MinScale * defaultScale;
        }
        public static float GetYouMinScale(float defaultScale, float minScale)
        {
            return minScale * defaultScale;
        }

        public static int ZIndexByY(int ZIndex, Vector2 Position)
        {
            ZIndex = (int)(GameContants.ScreenHalfH - Position.Y);
            if (ZIndex < 0) ZIndex = 0;
            return ZIndex;
        }

        public static Vector2 ScaleForYFar(float y, float minScale, float maxScale)
        {
            // y越小scale越小，最小minScale；y越大scale越大，最大maxScale。y可能为负数
            float screenH = GameContants.ScreenHalfH * 2f;
            // 将y归一化到0~1区间，y最小（负最大）时t=0，y最大时t=1
            float t = Mathf.Clamp((y + screenH / 2f) / screenH, 0f, 1f);
            float scaleVal = Mathf.Lerp(minScale, maxScale, t);
            // GD.Print($"ViewTool ScaleForYFar y: {y}, t: {t}, scaleVal: {scaleVal}");
            return new Vector2(scaleVal, scaleVal);
        }

        public static Vector2 FlipFace(Vector2 Scale, Vector2 Position)
        {
            if (Position.X > 0)
            {
                Scale = new Vector2(-Math.Abs(Scale.X), Scale.Y);
            }
            else
            {
                Scale = new Vector2(Math.Abs(Scale.X), Scale.Y);
            }
            return Scale;
        }


        public static void ViewZiFace(Node2D node)
        {
            int zi = node.ZIndex;
            Vector2 Scale = node.Scale;
            Vector2 Position = node.Position;
            // 缩 Y
            zi = ViewTool.ZIndexByY(zi, Position);
            node.ZIndex = zi;
            // Scale = ViewTool.ScaleForYFar(Position.Y, minScale, maxScale);
            // 默认朝左，x>0则翻转
            Scale = ViewTool.FlipFace(Scale, Position);
            node.Scale = Scale;
        }
        public static void View3In1(Node2D node, float minScale, float maxScale)
        {
            int zi = node.ZIndex;
            Vector2 Scale = node.Scale;
            Vector2 Position = node.Position;
            // 缩 Y
            zi = ViewTool.ZIndexByY(zi, Position);
            node.ZIndex = zi;
            Scale = ViewTool.ScaleForYFar(Position.Y, minScale, maxScale);
            // 默认朝左，x>0则翻转
            Scale = ViewTool.FlipFace(Scale, Position);
            node.Scale = Scale;
        }
        public static void View2In1(Node2D node, float minScale, float maxScale)
        {
            int zi = node.ZIndex;
            Vector2 Scale = node.Scale;
            Vector2 Position = node.Position;
            // 缩 Y
            zi = ViewTool.ZIndexByY(zi, Position);
            node.ZIndex = zi;
            Scale = ViewTool.ScaleForYFar(Position.Y, minScale, maxScale);
            // 默认朝左，x>0则翻转
            // Scale = ViewTool.FlipFace(Scale, Position);
            node.Scale = Scale;
        }

        public static Vector2 PianyiXDirection(Vector2 Position, Vector2 defaultDirection, float maxAngle)
        {
            float x = Position.X;
            float maxX = 400f; // 参考最大x
            float t = Mathf.Clamp(Mathf.Abs(x) / maxX, 0f, 1f);
            float angle = maxAngle * t;
            float angleOffset = 0f;
            if (x < 0)
            {
                angleOffset = Mathf.DegToRad(angle);
            }
            else if (x > 0)
            {
                angleOffset = Mathf.DegToRad(-angle);
            }
            if (angleOffset != 0f)
            {
                defaultDirection = defaultDirection.Rotated(angleOffset);
            }
            return defaultDirection;
        }

        public static float ScaleSpeedForScale(Vector2 Scale, float lowestSpeedScale = 0.5f)
        {
            float scale = Scale.X > 0 ? Scale.X : 0f;
            float speedScale = Mathf.Clamp(scale, lowestSpeedScale, 1f); // 0.3~1
            return speedScale;
        }
    }
}