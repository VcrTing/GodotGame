using Godot;
using System;
using ZVB4.Conf;

using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansCherry : Node2D, IWorking, IObj, IAttack
{
    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.Cherry;
    public string GetObjName() => objName;
    [Export]
    public int damage = 0;
    public int GetDamage() => damage;
    [Export]
    public int damageExtra = 0;
    public int GetDamageExtra() => damageExtra;
    [Export]
    public EnumMoveType moveType = EnumMoveType.Stone;
    public EnumMoveType GetEnumMoveType() => moveType;

    AnimatedSprite2D view;
    AnimatedSprite2D viewDoing;
    private GpuParticles2D _fx;
    private Area2D _attackArea;
    private bool _isWorkingMode = false;

    public bool Init(string name = null)
    {
        view = GodotTool.GetViewAndAutoPlay(this);
        viewDoing = GetNodeOrNull<AnimatedSprite2D>(NameConstants.ViewDoing);
        viewDoing.Visible = false;
        damage = DmageConstants.GetPlansDamage(objName);
        damageExtra = DmageConstants.GetPlansDamageExtra(objName);
        //
        _fx = GetNodeOrNull<GpuParticles2D>(NameConstants.Fx);
        if (_fx != null)
        {
            _fx.Emitting = false;
        }
        _attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
        if (_attackArea != null)
        {
            _attackArea.Visible = false;
            _attackArea.SetDeferred("monitoring", false);
        }
        return true;
    }

    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        AdjustView();
        Init();
    }
    public override void _Process(double delta)
    {
        AdjustView();
    }


    public bool IsWorkingMode;
    public void SetWorkingMode(bool working)
    {
        if (working) { Boom(); }
        IsWorkingMode = working;
    }

    void BeforeBoom()
    {
        view.Play(NameConstants.Boom);
    }
    void AfterBoom()
    {
        viewDoing.QueueFree();
        if (_attackArea != null)
        {
            _attackArea.QueueFree();
            _attackArea = null;
        }
        if (_fx != null)
        {
            _fx.QueueFree();
            _fx = null;
        }
    }
    public async void Boom()
    {
        BeforeBoom();
        // 1秒后爆炸
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        if (_fx != null)
        {
            _fx.Emitting = true;
        }
        if (_attackArea != null)
        {
            _attackArea.Visible = true;
            _attackArea.SetDeferred("monitoring", true);
        }
        if (view != null)
        {
            view.QueueFree();
            view = null;
        }

        viewDoing.Visible = true;
        viewDoing.Play(NameConstants.Default);
        // 播放音效
        try
        {
            SoundPlayerController.Instance.EnqueueSound("Plans/" + objName, objName, 5);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Error playing sound: {ex.Message}");
        }
        // 0.1秒后销毁
        await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
        AfterBoom();
        Die();
    }

    async void _DoingDie()
    {
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        QueueFree();
    }

    public bool Die()
    {
        _DoingDie();
        return true;
    }

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        return true;
    }

}
