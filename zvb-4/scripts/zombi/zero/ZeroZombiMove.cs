using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ZeroZombiMove : Node2D
{
    [Export]
    public Vector2 MoveDirection = new Vector2(0, 1); // 默认向下
    private Node2D _parentNode2D;
    private IMove _parentMove;
    private IStatus _parentStatus;
    float maxAngle = 10f; // 最大偏移角度

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
        // DDD
        // 根据初始x位置，MoveDirection稍微角度偏移
        MoveDirection = ViewTool.PianyiXDirection(_parentMove.GetMyPosition(), MoveDirection, maxAngle);

        IObj obj = GetParent() as IObj;
        ParentMoveSpeed = EnmyTypeConstans.GetSpeed(obj.GetObjName());
    }

    public override void _Process(double delta)
    {
        _moveTime += (float)delta;
        if (_parentMove != null)
        {
            Vector2 pos = _parentMove.GetMyPosition();
            switch (_parentMove.GetEnumMoveType())
            {
                case EnumMoveType.LineWalk:
                    pos = LineWalk(pos, delta);
                    break;
                case EnumMoveType.LineRun:
                    pos = LineRun(pos, delta);
                    break;
                default:
                    break;
            }
            _parentMove.SetMyPosition(pos);
        }
    }

    [Export]
    public float ParentMoveSpeed = 0f;
    private float _moveTime = 0f;
    private float _addSpeedTime = 4f;
    private float _minSpeedScale = 0.6f;


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

    public Vector2 LineWalk(Vector2 pos, double delta)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed, 1f);
        pos += MoveDirection.Normalized() * sp * (float)delta;
        return pos;
    }
    
    public Vector2 LineRun(Vector2 pos, double delta)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed, 2.5f);
        pos += MoveDirection.Normalized() * sp * (float)delta;
        return pos;
    }
}
