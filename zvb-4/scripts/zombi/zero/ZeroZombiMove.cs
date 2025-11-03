using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZeroZombiMove : Node2D
{
    public Vector2 MoveDirection = new Vector2(0, 1); // 默认向下
    private Node2D _parentNode2D;
    private IMove _parentMove;
    private IStatus _parentStatus;
    float maxAngle = 10f; // 最大偏移角度
    Vector2 PlayerPosition;
    int __random = 0;
    public override void _Ready()
    {
        var parent = GetParent();
        _parentNode2D = parent as Node2D;
        // 获取父节点上挂载的所有脚本，查找实现IMove的
        _parentMove = null;
        if (_parentNode2D != null)
        {
            _parentMove = parent as IMove;
            _parentStatus = parent as IStatus;
        }
        __random = GD.RandRange(0, 100);
        // 根据初始x位置，MoveDirection稍微角度偏移
        Vector2 dir = _parentMove.GetBasicDirection();
        MoveDirection = dir;
        if (dir.Y > dir.X)
        {
            MoveDirection = ViewTool.PianyiXDirection(_parentMove.GetMyPosition(), dir, maxAngle);
        }
        PlayerPosition = PlayerController.Instance.GetPlayerPosition();
        IObj obj = GetParent() as IObj;
        ParentMoveSpeed = EnmyTypeConstans.GetSpeed(obj.GetObjName());
    }

    float __fastTime = 0f;
    public override void _Process(double delta)
    {
        _moveTime += (float)delta;
        Vector2 pos = _parentMove.GetMyPosition();
        switch (_parentMove.GetEnumMoveType())
        {
            case EnumMoveType.LineWalk:
                pos = Move(pos, delta, AnimationConstants.MoveWalkRatio);
                break;
            case EnumMoveType.LineRun:
                pos = Move(pos, delta, AnimationConstants.MoveRunRatio);
                break;
            case EnumMoveType.LineWalkFast:
                pos = Move(pos, delta, AnimationConstants.MoveWalkFastRatio);
                break;
            case EnumMoveType.LineRunFast:
                __fastTime += (float)delta;
                pos = LineRunFastQuXian(pos, delta, AnimationConstants.MoveRunFastRatio);
                break;
            case EnumMoveType.RunToPalyer:
                pos = RunToPlayer(pos, delta, AnimationConstants.MoveRunToPlayRatio);
                break;
            case EnumMoveType.WalkToPalyer:
                pos = RunToPlayer(pos, delta, AnimationConstants.MoveWalkToPlayRatio);
                break;
            default:
                break;
        }
        _parentMove.SetMyPosition(pos);
    }

    [Export]
    public float ParentMoveSpeed = 0f;
    private float _moveTime = 0f;
    private float _addSpeedTime = 4f;
    private float _minSpeedScale = 0.6f;

    // 随机一点移速
    public float GetRandomSpeedRatio()
    {
        float f = __random / 1000f;
        return f + 0.98f;
    }

    // 根据移动时机，曲线加速
    float QuXianJiaSu(float speed, float ratio)
    {
        // X 速度倍率
        speed = speed * ratio;
        // 计算速度
        float t = Mathf.Clamp(_moveTime / _addSpeedTime, 0f, 1f);
        float speedBase = Mathf.Lerp(speed * _minSpeedScale, speed, t);
        // 加速1~4f随机数
        speedBase += (float)GD.RandRange(1f, 4f * ratio);
        // 加入速度缩放
        float scale = _parentStatus.GetMoveSpeedScale();
        return speedBase * scale;
    }
    public Vector2 Move(Vector2 pos, double delta, float ratio)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed, ratio);
        pos += MoveDirection.Normalized() * sp * (float)delta * GetRandomSpeedRatio();
        return pos;
    }
    public float GetFastSpeedRatio()
    {
        if (__fastTime > 1) __fastTime = 1; 
        float f = __fastTime / 300f;
        return f + 1.001f;
    }
    public Vector2 LineRunFastQuXian(Vector2 pos, double delta, float ratio)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed, ratio);
        pos += MoveDirection.Normalized() * sp * (float)delta * GetRandomSpeedRatio() * GetFastSpeedRatio();
        return pos;
    }
    //
    void RefreshDirection(Vector2 pos)
    {
        MoveDirection = PlayerPosition - pos;
    }
    public Vector2 RunToPlayer(Vector2 pos, double delta, float ratio)
    {
        RefreshDirection(pos);
        return Move(pos, delta, ratio);
    }
}
