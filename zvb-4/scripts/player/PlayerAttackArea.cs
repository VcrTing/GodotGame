using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

enum EnumOptionType
{
    None,
    EmptyTouch,
    CatchPlans,
    TouchUx,
}

public partial class PlayerAttackArea : Area2D
{
    TileMapLayer PlansWorkingTileMap;
    private bool _isPressing = false; // 鼠标或触摸是否按下
    private Vector2 _lastDragPos = Vector2.Zero;
    private bool _isPointerInside = false;
    private bool _isActive = false; // 是否处于“持续调用”状态
    EnumOptionType optionType = EnumOptionType.None;
    EnumOptionType lastOptionType = EnumOptionType.None;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        MouseEntered += OnPointerEnter;
        MouseExited += OnPointerExit;
        PlansWorkingTileMap = GetNodeOrNull<TileMapLayer>(NameConstants.PlansWorkingTileMap);
        SetProcessInput(true);
        SetProcess(true);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseBtn)
        {
            if (mouseBtn.ButtonIndex == MouseButton.Left)
            {
                if (mouseBtn.Pressed)
                {
                    _isPressing = true;
                    _lastDragPos = this.GetLocalMousePosition(); // mouseBtn.Position;
                    if (_isPointerInside)
                    {
                        _isActive = true;
                        OnDragOrTouch(_lastDragPos);
                    }
                }
                else // 松开
                {
                    if (_isPointerInside && _isActive)
                    {
                        OnRelease(_lastDragPos);
                    }
                    _isPressing = false;
                    _isActive = false;
                }
            }
        }
        else if (@event is InputEventScreenTouch touch)
        {
            if (touch.Pressed)
            {
                _isPressing = true;
                _lastDragPos = this.GetLocalMousePosition(); // touch.Position;
                if (_isPointerInside)
                {
                    _isActive = true;
                    OnDragOrTouch(_lastDragPos);
                }
            }
            else // 抬起
            {
                if (_isPointerInside && _isActive)
                {
                    OnRelease(_lastDragPos);
                }
                _isPressing = false;
                _isActive = false;
            }
        }
        else if (@event is InputEventMouseMotion motion && _isPressing)
        {
            _lastDragPos = this.GetLocalMousePosition(); // motion.Position;
        }
        else if (@event is InputEventScreenDrag drag && _isPressing)
        {
            _lastDragPos = this.GetLocalMousePosition(); // drag.Position;
        }
    }

    public override void _Process(double delta)
    {
        if (_isActive)
        {
            OnDragOrTouch(_lastDragPos);
        }
    }
    private void OnPointerEnter()
    {
        _isPointerInside = true;
        if (_isPressing)
        {
            _isActive = true;
            OnDragOrTouch(_lastDragPos);
        }
    }
    private void OnPointerExit()
    {
        _isPointerInside = false;
        _isActive = false;
    }

    public void PlayerAttacking(Vector2 worldPos)
    {
        __PlayerController(worldPos, false);
    }
    public void PlayerAttackRelease(Vector2 worldPos, bool isClear)
    {
        __PlayerController(worldPos, true);
    }
    void __PlayerController(Vector2 worldPos, bool isRelease = false, bool isClear = false)
    {
        try
        {
            var playerController = PlayerController.Instance;
            if (playerController != null)
            {
                if (isRelease)
                {
                    if (isClear)
                    {
                        playerController.ReleaseAttackClear(worldPos, true);
                    }
                    else
                    {
                        playerController.ReleaseAttack(worldPos);
                    }
                }
                else
                {
                    playerController.TryAttack(worldPos);
                }
            }
        }
        catch { }
    }

    // 持续拖拽/触摸时调用的方法
    private void OnDragOrTouch(Vector2 pos)
    {
        // 攻击
        PlayerAttacking(this.ToGlobal(pos));
    }
    // 区域内松开/抬起时调用的方法
    private void OnRelease(Vector2 pos)
    {
        // 攻击
        PlayerAttackRelease(this.ToGlobal(pos), false);
    }
    void OnAreaExited(Area2D area)
    {
        string name = area.Name;
        if (name == NameConstants.MiaoArea)
        {
            lastObj = null;
            lastNode = null;
        }
    }
    //
    IObj lastObj = null;
    Node2D lastNode = null;
    Vector2 lastPos = Vector2.Up;
    //
    private void OnAreaEntered(Area2D area)
    {
        string name = area.Name;
        if (name == NameConstants.MiaoArea)
        {
            PlayerAttackRelease(lastPos, true);
            IObj obj = area.GetParent() as IObj;
            if (obj != null)
            {
                lastObj = obj;
                lastNode = area.GetParent<Node2D>();
            }
        }
    }
    /*
    void DieToReword()
    {
        var table = ShooterWorkTable.Instance;
        if (table == null) return;
        table.DieToReword(lastNode);
    }

    void DieLastObj()
    {
        if (lastObj != null)
        {
            ReleaseFlowerPeng();
            PlayerAttackRelease(lastPos, true);
            ResetOptionType();
            lastObj.Die();
            lastObj = null;
            lastNode = null;
        }
    }

    bool IsShooter(string planName)
    {
        bool isOk = false;
        if (planName == PlansConstants.Pea || planName == PlansConstants.XiguaBing || planName == PlansConstants.YangTao)
        {
            isOk = true;
        }
        return isOk;
    }

    bool WorkForPlans(Vector2 pos, string planName)
    {
        bool isOk = false;
        // Pea
        if (IsShooter(planName))
        {
            // 错误
            DieToReword();
            DieLastObj();
        }
        else
        {
            // 一般植物
            isOk = plansPlanting.ZhongZhiPlans(pos, planName);
            if (isOk)
            {
                DieLastObj();
            }
        }
        return isOk;
    }

    void ReleaseFlowerPeng()
    {
        FlowerPeng flowerPeng = lastNode.GetParent<FlowerPeng>();
        if (flowerPeng != null)
        {
            flowerPeng.ReleaseLock();
        }
    }
    void ChangeOptionType(EnumOptionType newType)
    {
        if (optionType != newType)
        {
            lastOptionType = optionType;
            optionType = newType;
        }
    }
    void TurnBackOptionType(EnumOptionType nowType)
    {
        optionType = lastOptionType;
        lastOptionType = nowType;
    }

    void ResetOptionType()
    {
        lastOptionType = EnumOptionType.None;
        optionType = EnumOptionType.None;
    }
    */

}
