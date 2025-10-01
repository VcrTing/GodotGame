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

    //
    IPlansPlanting plansPlanting;

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;

        MouseEntered += OnPointerEnter;
        MouseExited += OnPointerExit;

        PlansWorkingTileMap = GetNodeOrNull<TileMapLayer>(NameConstants.PlansWorkingTileMap);
        SetProcessInput(true);
        SetProcess(true);
        ResetOptionType();
        plansPlanting = PlansWorkingTileMap as IPlansPlanting;
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
        // GD.Print("PlayerAttackArea 鼠标进入");
        _isPointerInside = true;
        // 只有在按下状态下，滑入才激活持续调用
        if (_isPressing)
        {
            _isActive = true;
            OnDragOrTouch(_lastDragPos);
        }
    }
    private void OnPointerExit()
    {
        // GD.Print("PlayerAttackArea 鼠标离开");
        _isPointerInside = false;
        _isActive = false;
    }

    void PlayerAttackDoing(Vector2 worldPos)
    {
        __PlayerController(worldPos, false);
    }
    void PlayerAttackRelease(Vector2 worldPos, bool isClear)
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
        var worldPos = this.ToGlobal(pos);
        // TODO: 替换为你需要持续调用的逻辑
        // GD.Print($"持续拖拽/触摸: {pos}");
        lastPos = worldPos;
        if (optionType == EnumOptionType.CatchPlans)
        {
            // GD.Print($"持续拖拽/触摸: {pos} CatchPlans");
        }
        else
        {
            // 攻击
            PlayerAttackDoing(worldPos);
            //
            ChangeOptionType(EnumOptionType.EmptyTouch);
        }
    }

    // 区域内松开/抬起时调用的方法
    private void OnRelease(Vector2 pos)
    {
        var worldPos = this.ToGlobal(pos);
        lastPos = worldPos;
        // TODO: 替换为你需要的松开逻辑
        // GD.Print($"区域内松开/抬起: {pos} CatchPlans {GetLocalMousePosition()} {worldPos}");
        if (optionType == EnumOptionType.CatchPlans)
        {
            if (lastObj != null)
            {
                // 种植物
                WorkForPlans(worldPos, lastObj.GetObjName());
            }
        }
        PlayerAttackRelease(worldPos, false);
        ResetOptionType();
    }

    void OnAreaExited(Area2D area)
    {
        string name = area.Name;
        if (name == NameConstants.MiaoArea)
        {
            ResetOptionType();
            // TurnBackOptionType(EnumOptionType.CatchPlans);
            lastObj = null;
            lastNode = null;
        }
    }

    IObj lastObj = null;
    Node2D lastNode = null;
    Vector2 lastPos = Vector2.Up;

    private void OnAreaEntered(Area2D area)
    {
        string name = area.Name;
        if (name == NameConstants.MiaoArea)
        {
            PlayerAttackRelease(lastPos, true);
            ChangeOptionType(EnumOptionType.CatchPlans);
            IObj obj = area.GetParent() as IObj;
            if (obj != null)
            {
                lastObj = obj;
                lastNode = area.GetParent<Node2D>();
            }
        }
    }

    void DieToReword()
    {
        // 生成奖励组
        var rewordGroupScene = GD.Load<PackedScene>(FolderConstants.WaveObj + "reword_group.tscn");
        var rewordGroupNode = rewordGroupScene.Instantiate<Node2D>();
        GetParent().AddChild(rewordGroupNode);
        var rewordGroup = rewordGroupNode as RewordGroup;
        if (rewordGroup != null)
        {
            Vector2 pos = lastNode != null ? lastNode.Position : this.Position;
            // 生成阳光奖励
            rewordGroup.SpawnReword(SunMoneyConstants.Sun, SunMoneyConstants.GetSunNumNormal(lastObj.GetObjName()), pos, SunMoneyConstants.SunNormal);
            // 销毁植物
            SoundFxController.Instance?.PlayFx("Ux/trash", "trash", 4, pos);
        }
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

    bool WorkForPlans(Vector2 pos, string planName)
    {
        bool isOk = false;
        // Pea
        if (planName == PlansConstants.Pea)
        {
            // 错误
            DieToReword();
            DieLastObj();
        }
        // 西瓜冰
        else if (planName == PlansConstants.XiguaBing)
        {
            // 错误
            DieToReword();
            DieLastObj();
        }
        else
        {
            // 一般植物
            // GD.Print($"区域内松开/抬起: {pos} CatchPlans {GetLocalMousePosition()}");
            isOk = plansPlanting.ZhongZhiPlans(pos, planName);
            DieLastObj();
            // 解开花盆
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

}
