using Godot;
using System;
using ZVB4.Conf;

public partial class ChoiseShooterDetailView : Control
{
    public static ChoiseShooterDetailView Instance { get; private set; }

    public override void _Ready() { Instance = this; AsyncShooterView(); }
    
    public async void AsyncShooterView()
    {
        var sys = SaveDataManager.Instance;
        if (sys == null)
        {
            GetTree().CreateTimer(0.2f).Timeout += () => AsyncShooterView();
            return;
        }
        string shooterName = SaveDataManager.Instance.GetLastBaseShooter();
        if (shooterName != null)
        {
            I18nConstants.ShowTuJianPlansCard(this, shooterName, 120f, 90f, 1.5f);
        }
    }
}
