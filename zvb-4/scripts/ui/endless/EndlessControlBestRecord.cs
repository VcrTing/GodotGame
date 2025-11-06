using Godot;
using System;
using ZVB4.Entity;

public partial class EndlessControlBestRecord : Control
{
    Label labelSunRecord;
    Label labelMoneyRecord;
    Label labelTime;
    Label labelEnmyWave;

    public override void _Ready()
    {
        labelSunRecord = GodotTool.FindLabelByName(this, "LabelSunRecord");
        labelMoneyRecord = GodotTool.FindLabelByName(this, "LabelMoneyRecord");
        labelTime = GodotTool.FindLabelByName(this, "LabelTime");
        labelEnmyWave = GodotTool.FindLabelByName(this, "LabelEnmyWave");
    }

    public void AsyncLabelView(EntityEndlessBestRecord record)
    {
        labelSunRecord.Text = record.SunTotalGet + "";
        labelMoneyRecord.Text = record.MoneyTotalGet + "";
        labelEnmyWave.Text = record.EnmyWave + "";
        labelTime.Text = record.Timed + "";
    }
}
