using Godot;
using System;
using ZVB4.Conf;

using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansMoRiGu : Node2D, IWorking, IObj, IAttack
{
    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.MoRiGu;
    public string GetObjName() => objName;
    [Export]
    public int damage = 0;
    public int GetDamage() => damage;
    [Export]
    public int damageExtra = 0;
    public int GetDamageExtra() => damageExtra;
    // 
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
        damage = ObjTool.GetObjDamage(EnumObjType.Plans, objName);
        damageExtra = ObjTool.GetObjDamageExtra(EnumObjType.Plans, objName);
        //
        _fx = GetNodeOrNull<GpuParticles2D>(NameConstants.Fx);
        if (_fx != null) { _fx.Emitting = false; }
        ChangeAttackArea(false);
        return true;
    }
    void ChangeAttackArea(bool active)
    {
        try
        {
            if (_attackArea == null) _attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
            if (_attackArea != null)
            {
                _attackArea.Visible = active;
                _attackArea.SetDeferred("monitoring", active);
            }
        }
        catch { }
    }
    public override void _Ready()
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        Init();
    }
    float __t = 0f;
    public override void _Process(double delta)
    {
        ViewTool.View3In1(this, minScale, maxScale);
        if (__t > 0f)
        {
            __t += (float)delta;
            if (__t > 1f)
            {
                Boom();
                if (__t > 1.4f)
                {
                    AfterBoom();
                    __t = 0f;
                }
            }
        }
    }
    public bool IsWorkingMode;
    public void SetWorkingMode(bool working)
    {
        if (working) { StartBoom(); }
        IsWorkingMode = working;
    }
    void AfterBoom()
    {
        viewDoing.QueueFree();
        ChangeAttackArea(false);
        if (_fx != null)
        {
            _fx.QueueFree();
            _fx = null;
        }
        Die();
    }
    public void StartBoom() { AnimationTool.DoAniAttack(view); __t = 0.00001f; }
    bool isBoom = false;
    public void Boom()
    {
        if (isBoom) return; isBoom = true;
        if (_fx != null)
        {
            _fx.Emitting = true;
        }
        ChangeAttackArea(true);
        if (view != null)
        {
            view.QueueFree();
            view = null;
        }
        viewDoing.Visible = true;
        viewDoing.Play(NameConstants.Default);
        SoundPlayerController.Instance.EnqueueSound("Plans/" + objName, objName, 4);
    }
    public bool Die()
    {
        QueueFree();
        return true;
    }
    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    public bool IsWorking() => IsWorkingMode;
    public bool CanAttack() => true;
}
