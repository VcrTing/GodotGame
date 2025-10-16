
using Godot;
using ZVB4.Conf;
using ZVB4.Interface;

public static class ZombiTool
{

    public static bool RunningWhenDie(Node2D me, Node2D bodyNode)
    {
        IObj obj = me as IObj;
        //
        GameStatistic.Instance?.AddZombieDead();
        // 关闭碰撞
            Node2D beHurtArea = me.GetNodeOrNull<Node2D>(NameConstants.BeHurtArea);
        if (beHurtArea != null)
        {
            beHurtArea.QueueFree();
        }
        // 删掉移动
        Node2D m = me.GetNodeOrNull<Node2D>(NameConstants.Move);
        if (m != null)
        {
            m.QueueFree();
        }
        // 掉落奖励
        IWhenDie wdr = me.GetNodeOrNull<IWhenDie>(NameConstants.WorkingDumpReword);
        if (wdr != null)
        {
            wdr.WorkingWhenDie(obj.GetObjName());
        }

        return true;
    }
}