using Godot;
using System;
using ZVB4.Conf;

public partial class SunCenterSystem : Control
{
    private int _value = 100;
    private Label _sunShowLabel;

    public static SunCenterSystem Instance { get; private set; }

    public int Value => _value;

    public override void _Ready()
    {
        Instance = this;
        // _sunShowLabel = GetNodeOrNull<Label>(NameConstants.SunShowLabel);
        UpdateSunLabel();
    }

    public void SetValue(int v)
    {
        _value = v;
        UpdateSunLabel();
    }

    void LoadLabel()
    {
        if (_sunShowLabel == null)
        {
            _sunShowLabel = GodotTool.FindLabelByName(this, NameConstants.SunShowLabel);
        }
    }

    public Vector2 GetLabelPosition()
    {
        LoadLabel();
        if (_sunShowLabel != null)
        {
            // return _sunShowLabel.GlobalPosition;
        }
        return new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH + 90f);
    }
    // 增加 value
    public void AddValue(int v)
    {
        _value += v;
        UpdateSunLabel();
    }

    // 减少 value，不能为负
    public void SubValue(int v)
    {
        _value -= v;
        if (_value < 0)
            _value = 0;
        UpdateSunLabel();
    }

    private void UpdateSunLabel()
    {
        LoadLabel();
        if (_sunShowLabel != null)
        {
            _sunShowLabel.Text = _value.ToString();
        }
    }

    public bool NextSunIsZero(int v)
    {
        int _v = _value - v;
        return _v < 0;
    }

    // 增加普通阳光
    public void FlowerGet()
    {
        _value += SunMoneyConstants.FlowerGetSun;
    }
    public void SkyGet()
    {
        _value += SunMoneyConstants.SkyGetSun;
    }

    // 攻击扣除
    public void ForAttack(int v)
    {
        SubValue(v);
    }
}
