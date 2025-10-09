using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class PlansIceFlower : Node2D, IObj, IWorking, IAttack
{
    Area2D attackArea;
    AnimatedSprite2D view;

    [Export]
    public float startWorkTime = 1.2f;
    //
    public override async void _Ready()
    {
        attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        attackArea.Monitoring = false;
        //
    }
    float __processTime = 0f;
    public override void _Process(double delta)
    {
        if (__processTime <= 0f) return;
        __processTime += (float)delta;
        if (__processTime > (1f + startWorkTime))
        {
            if (!_isFadingOut)
            {
                _isFadingOut = true;
                _fadeElapsed = 0f;
            }
        }
        if (_isFadingOut)
        {
            _fadeElapsed += (float)delta;
            float t = Mathf.Clamp(_fadeElapsed / 0.5f, 0f, 1f);
            if (view != null)
            {
                var c = view.Modulate;
                view.Modulate = new Color(c.R, c.G, c.B, 1f - t);
            }
            if (_fadeElapsed >= 0.5f)
            {
                QueueFree();
            }
        }
    }
    private bool _isFadingOut = false;
    private float _fadeElapsed = 0f;
    //
    public void OnArea2DEntered(Area2D area2D)
    {
        IHurtBase ist = area2D as IHurtBase;
        if (ist != null)
        {
            ist.TakeDamage(objType, 0, enumHurts);
        }
    }
    EnumHurts enumHurts = EnumHurts.IceFreeze;
    EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType()
    {
        return objType;
    }
    string objName = PlansConstants.IceFlower;
    public string GetObjName()
    {
        return objName;
    }

    public bool Init(string name = null)
    {
        return true;
    }

    public bool Die()
    {
        return true;
    }
    public void DieStar()
    {
        view.Play(AnimationConstants.AniOutro);
    }

    public bool AdjustView()
    {
        throw new NotImplementedException();
    }

    async void Working()
    {
        __processTime = 0.001f; // 开始计时
        await ToSignal(GetTree().CreateTimer(startWorkTime), "timeout");
        attackArea.AreaEntered += OnArea2DEntered;
        attackArea.Monitoring = true;
        // 播放音效
        SoundFxController.Instance?.PlayFx("Plans/" + objName, objName, 4);
    }

    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        if (working)
        {
            view.Visible = true;
            view.Play(AnimationConstants.AniWorking);
            Working();
        }
    }

    public int GetDamage()
    {
        return 0;
    }

    public int GetDamageExtra()
    {
        return 0;
    }

    public bool IsWorking() => isWorking;
}
