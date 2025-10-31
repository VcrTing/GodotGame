
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

    public static bool CanBeCold(EnumWhatYouObj whatYouObj, EnumHurts enumHurts)
    {
        if (enumHurts == EnumHurts.ColdZheng) return true;

        if (enumHurts == EnumHurts.Cold)
        {
            // 钢门 / 报纸，不能被Cold减速
            if (whatYouObj == EnumWhatYouObj.GangMen || whatYouObj == EnumWhatYouObj.Baozhi)
            {
                GD.Print("钢门报纸 不减速 COld");
                return false;
            }
            return true;
        }
        return true;
    }
    
    public static bool IsOnlyHurtBody(EnumWhatYouObj whatYouObj, EnumHurts enumHurts)
    {
        // 钢门
        if (whatYouObj == EnumWhatYouObj.GangMen)
        {
            // 针，通过
            if (enumHurts == EnumHurts.Zheng || enumHurts == EnumHurts.ColdZheng)
            {
                GD.Print("针 / 冰针 通过钢门");
                return true;
            }
            // 泡泡，通过
            if (enumHurts == EnumHurts.PaoPao)
            {
                GD.Print("泡泡 通过钢门");
                return true;
            }
        }
        // 报纸
        if (whatYouObj == EnumWhatYouObj.Baozhi)
        {
            // 只有针，通过
            if (enumHurts == EnumHurts.Zheng || enumHurts == EnumHurts.ColdZheng)
            {
                GD.Print("针 / 冰针 通过报纸");
                return true;
            }
        }

        return false;
    }
}