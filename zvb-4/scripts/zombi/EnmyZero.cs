using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class EnmyZero : Node2D, IHurtBase
{
    public Vector2 Direction { get; set; } = Vector2.Down; // 默认向下
    
    [Export]
    public EnumHealth HealthType { get; set; } = EnumHealth.Two;

    int Health { get; set; } = 100;
    // public int Attack { get; set; } = 10;
    // public float AttackSpeed { get; set; } = 1.0f; // 每秒攻击一次
    // private float _attackCooldown = 0f;

    private float _moveTimer = 0f;
    private float _moveInterval = 2f; // 每2f秒移动一次

    private float _moveStep = GameContants.TileW * GameContants.GrassNumHalf; // 每次移动的距离，可根据需要调整

    private float _currentSpeed = 0f;
    private float _acceleration = 100f; // 加速度
    private float _deceleration = 300f; // 减速度
    private float _maxSpeed = 0f;
    private bool _isMoving = false;
    private float _moveElapsed = 0f;
    public float Speed { get; set; } = 30f;

    public override void _Process(double delta)
    {
        _moveTimer += (float)delta;
        if (_moveTimer >= _moveInterval)
        {
            _moveTimer = 0f;
            _isMoving = true;
            _moveElapsed = 0f;
            _currentSpeed = 0f;
            _maxSpeed = Speed;
        }
        if (_isMoving)
        {
            _moveElapsed += (float)delta;
            float halfInterval = _moveInterval / 2f;
            if (_moveElapsed < halfInterval)
            {
                // 加速阶段
                _currentSpeed = Mathf.Min(_currentSpeed + _acceleration * (float)delta, _maxSpeed);
            }
            else if (_moveElapsed < _moveInterval)
            {
                // 减速阶段
                _currentSpeed = Mathf.Max(_currentSpeed - _deceleration * (float)delta, 0f);
            }
            else
            {
                _isMoving = false;
                _currentSpeed = 0f;
                AfterWalk();
            }
            Position += Direction * _currentSpeed * (float)delta;
        }
    }

    public void AfterWalk()
    {
        // 步行后给_moveInterval一个1.2-3之间的随机值
        _moveInterval = GD.RandRange(100, 400)/100f;
        float s = GD.RandRange(30, 50)/10f;
        _moveStep = (float)(s * GD.RandRange(GameContants.TileW - 20f, GameContants.TileW + 20f)); // 每次移动的距离，可根据需要调整
        // 加速度随机
        _acceleration = (float)GD.RandRange(80f, 150f);
        _deceleration = (float)GD.RandRange(200f, 400f);
        // 
        Speed = (float)GD.RandRange(2, 12);
    }

    public override void _Ready()
    {
        Health = (int)HealthType;
        AfterWalk();
    }

    public bool TakeDamage(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        Health -= damage;
        SoundGameObjController.PlayBeHurtFx(objType, damage, enumHurts, EnumWhatYouObj.ZombiSoftBody, this.GlobalPosition);
        if (Health <= 0)
        {
            Die();
        }
        else
        {
            WhenBeHurtNoDie();
        }
        return true;
    }

    async void WhenBeHurtNoDie()
    {
        // 受击反馈
        var view = GetNodeOrNull<CanvasItem>(NameConstants.View);
        if (view != null)
        {
            view.Modulate = new Color(1, 1, 1, 0.618f);
            await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
            view.Modulate = new Color(1, 1, 1, 1f);
        }
    }

    void DieFaker() {
        // 获取自身节点并设置透明度为0
        var sprite = GetNodeOrNull<Node2D>(".");
        if (sprite != null)
        {
            var modulateProp = sprite.GetType().GetProperty("Modulate");
            if (modulateProp != null)
            {
                modulateProp.SetValue(sprite, new Color(1, 1, 1, 0));
            }
            else if (sprite is CanvasItem ci)
            {
                ci.Modulate = new Color(1, 1, 1, 0);
            }
        }
    }

    void DieForBody()
    {
        // 获取BeHurtArea并禁用碰撞
        var beHurtArea = GetNodeOrNull<Area2D>(NameConstants.BeHurtBody);
        if (beHurtArea != null)
        {
            beHurtArea.Monitoring = false;
            beHurtArea.SetDeferred("monitorable", false);
        }
        // 获取AttackArea并禁用碰撞
        var attckBody = GetNodeOrNull<Area2D>(NameConstants.AttackBody);
        if (attckBody != null)
        {
            attckBody.Monitoring = false;
            attckBody.SetDeferred("monitorable", false);
        }
    }

    private async void Die()
    {
        DieFaker();
        // 禁用移动
        _moveInterval = float.MaxValue;
        DieForBody();
        // 0.5秒后销毁
        await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        QueueFree();
    }
}
