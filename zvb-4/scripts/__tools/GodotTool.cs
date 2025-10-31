using System;
using System.Collections.Generic;
using Godot;
using ZVB4.Conf;

public static class GodotTool
{
    // 递归查找指定名称的Label
    public static Label FindLabelByName(Node node, string name)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Label label && child.Name == name)
                return label;
            var found = FindLabelByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    // 递归查找指定名称的CanvasItem
    public static CanvasItem FindCanvasItemByName(Node node, string name)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is CanvasItem canvasItem && child.Name == name)
                return canvasItem;
            var found = FindCanvasItemByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    // 递归查找指定名称的Node2D
    public static Node2D FindNode2DByName(Node node, string name)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Node2D node2d && child.Name == name)
                return node2d;
            var found = FindNode2DByName(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    public static void SwitchOneVisible(Node2D node, string name)
    {
        if (node == null) return;

        // 隐藏所有子节点
        foreach (Node child in node.GetChildren())
        {
            if (child is CanvasItem ci)
                ci.Visible = false;
        }
        // 获取指定名称的子节点并显示
        var target = node.GetNodeOrNull<CanvasItem>(name);
        if (target != null)
        {
            target.Visible = true;
        }
    }
    public static void SwitchAnimatedSprite(Node2D node, string viewName, string animationName = "default")
    {
        if (node == null) return;

        // 隐藏所有子节点
        foreach (Node child in node.GetChildren())
        {
            if (child is CanvasItem ci)
                ci.Visible = false;
        }
        // 获取指定名称的子节点并播放动画

        var animatedSprite = node.GetNodeOrNull<AnimatedSprite2D>(viewName);
        if (animatedSprite != null)
        {
            animatedSprite.Visible = true;
            animatedSprite.Play(animationName);
        }
    }

    //
    public static
    AnimatedSprite2D GetViewAndAutoPlay(Node2D node)
    {
        AnimatedSprite2D view = node.GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        if (view == null)
        {
            view = node as AnimatedSprite2D;
        }
        if (view != null)
        {
            view.Visible = true;
            view.Play(NameConstants.Default);
        }
        return view;
    }

    public static void KillChildren(Node node)
    {
        try
        {
            var ass = node.GetChildren();
            for (int j = 0; j < ass.Count; j++)
            {
                Node a = ass[j];
                a.QueueFree();
            }
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
    }
    
    public static List<Control> GenerateControlList(Node father, int num, string scenepath)
    {
        List<Control> res = new List<Control>();
        foreach (var child in father.GetChildren())
        {
            if (child is Node node)
            {
                node.QueueFree();
            }
        }
        int i = 0;
        while (i < num)
        {
            var scene = GD.Load<PackedScene>(scenepath); // FolderConstants.Scenes + "card/in_game_store_item_card.tscn"
            if (scene == null) continue;
            var card = scene.Instantiate<Control>();
            res.Add(card);
            i += 1;
        }
        return res;
    }
}
