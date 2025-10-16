using Godot;
using System;
using ZVB4.Conf;

public partial class SunCenterSystem : Control
{
    private int _value = 100;
    private Label _countShowLabel;

    public static SunCenterSystem Instance { get; private set; }

    public int Value => _value;

    public override void _Ready()
    {
        Instance = this;
        // _countShowLabel = GetNodeOrNull<Label>(NameConstants.SunShowLabel);
        UpdateSunLabel();
    }

    public void SetValue(int v)
    {
        _value = v;
        UpdateSunLabel();
    }

    void LoadLabel()
    {
        if (_countShowLabel == null)
        {
            _countShowLabel = GodotTool.FindLabelByName(this, NameConstants.CountShowLabel);
        }
    }

    public Vector2 GetLabelPosition()
    {
        LoadLabel();
        if (_countShowLabel != null)
        {
            // return _countShowLabel.GlobalPosition;
        }
        return new Vector2(-GameContants.ScreenHalfW, -GameContants.ScreenHalfH + 90f);
    }
    // 增加 value
    public void AddValue(int v)
    {
        _value += v;
        UpdateSunLabel();
        GameStatistic.Instance?.AddSunProduced(v);
    }

    // 减少 value，不能为负
    public void SubValue(int v)
    {
        _value -= v;
        if (_value < 0)
            _value = 0;
        UpdateSunLabel();
        GameStatistic.Instance?.AddSunConsumed(v);
    }

    private void UpdateSunLabel()
    {
        LoadLabel();
        if (_countShowLabel != null)
        {
            _countShowLabel.Text = _value.ToString();
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
    public bool CostForAttack(int v)
    {
        bool isZero = NextSunIsZero(v);
        if (isZero)
        {
            // 错误音效
            SoundUiController.Instance?.Error();
            return false;
        }
        else
        {
            // 扣除阳光
            SubValue(v);
        }
        return true;
    }
}
