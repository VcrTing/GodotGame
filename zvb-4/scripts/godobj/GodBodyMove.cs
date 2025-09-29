using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class GodBodyMove : CharacterBody2D
{
    [Export]
    public Vector2 MoveDirection = new Vector2(0, 1); // 默认向下
    private Node2D _parentNode2D;
    private IMove _parentMove;
    float maxAngle = 10f; // 最大偏移角度

    public override void _Ready()
    {
        var parent = GetParent();
        _parentNode2D = parent as Node2D;
        // 获取父节点上挂载的所有脚本，查找实现IMove的
        _parentMove = null;
        if (_parentNode2D != null)
        {
            // Try to cast the parent node itself to IMove
            _parentMove = parent as IMove;
        }
        // DDD
        // 根据初始x位置，MoveDirection稍微角度偏移
        MoveDirection = ViewTool.PianyiXDirection(_parentNode2D.Position, MoveDirection, maxAngle);

        IObj obj = GetParent() as IObj;
        ParentMoveSpeed = EnmyTypeConstans.GetSpeed(obj.GetObjName());
    }

    public override void _Process(double delta)
    {
        _moveTime += (float)delta;
        if (_parentMove != null)
        {
            Vector2 pos = _parentNode2D.Position;
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
    float QuXianJiaSu(float speed)
    {
        float t = Mathf.Clamp(_moveTime / _addSpeedTime, 0f, 1f);
        float speedBase = Mathf.Lerp(speed * _minSpeedScale, speed, t);
        return speedBase;
    }

    public Vector2 LineWalk(Vector2 pos, double delta)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed);
        // 加速1~4f随机数
        sp += (float)GD.RandRange(1f, 4f);
        pos += MoveDirection.Normalized() * sp * (float)delta;
        return pos;
    }
    
    public Vector2 LineRun(Vector2 pos, double delta)
    {
        float sp = QuXianJiaSu(ParentMoveSpeed * 2);
        // 加速1~8f随机数
        sp += (float)GD.RandRange(1f, 8f);
        pos += MoveDirection.Normalized() * sp * (float)delta;
        return pos;
    }
}
