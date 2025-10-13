using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;


public partial class EnmyWorkingDumpReword : Node2D, IWhenDie
{
    float baseLv = 0;
    public void WorkingWhenDie(string n)
    {
        bool has = TryDumpMoney(n, false);
        has = TryDumpPlansMiao(n, false);
    }

    public bool TryDumpMoney(string n, bool prevSucc)
    {
        float v = RewordConstants.GetEnmyDumpMoneyLv(n);
        if (RewordConstants.ComputedYesNo(v, prevSucc, RewordConstants.MoneyPrevSuccRewordSubLv))
        {
            DumpMoney();
            return true;
        }
        return false;
    }

    public bool TryDumpPlansMiao(string n, bool prevSucc)
    {
        float v = RewordConstants.GetEnmyDumpMiaoLv(n);
        if (RewordConstants.ComputedYesNo(v, prevSucc, RewordConstants.MiaoPrevSuccRewordSubLv))
        {
            DumpPlansMiao();
            return true;
        }
        return false;
    }

    //
    void DumpPlansMiao()
    {
        var ms = RewordMiaoCenterSystem.Instance;
        if (ms != null)
        {
            string m = ms.GetRandomPlansNameWithPowerWeight();
            if (m == "") return;
            ms.DumpPlansMiao(GlobalPosition, m);
        }
    }
    void DumpMoney()
    {
        // 实际逻辑
        var ms = MoneyCenterSystem.Instance;
        if (ms != null)
        {
            ms.DumpMoney(GlobalPosition, "", false);
        }
    }
}
