using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class RewordGroup : Node2D
{
    private float _lifeTime = 0f;
    private float _checkTimer = 0f;
    private float _checkInterval = 0.5f;
    private bool _started = false;

    public override void _Ready()
    {
        // 延迟0.5s后开始检测
        _checkTimer = -_checkInterval; // 先等待0.5s
        _lifeTime = 0f;
        _started = true;
        
    }

    public override void _Process(double delta)
    {
        if (!_started) return;
        _lifeTime += (float)delta;
        _checkTimer += (float)delta;
        if (_checkTimer >= _checkInterval)
        {
            _checkTimer = 0f;
            if (GetChildCount() == 0)
            {
                Die();
                return;
            }
        }
        if (_lifeTime > 10f)
        {
            Die();
        }
    }

    void Die()
    {
        QueueFree();
    }


    // 批量生成阳光
    public void SpawnReword(string rewordName, int count, Vector2 startPos, int value)
    {
        float spacing = 30f;
        for (int i = 0; i < count; i++)
        {
            float x = startPos.X + i * spacing;
            Vector2 pos = new Vector2(x, startPos.Y);
            //
            Gen(rewordName, i, pos, value);
        }

    }

    async void Gen(string rewordName, int i, Vector2 pos, int value)
    {
        string scenePath = FolderConstants.WaveObj + rewordName + ".tscn";
        PackedScene sunScene = GD.Load<PackedScene>(scenePath);
        float delay = i * 0.1f;
        if (delay > 0f)
        {
            await ToSignal(GetTree().CreateTimer(delay), "timeout");
        }
        // x加1-5f随机偏移
        float randOffset = (float)GD.RandRange(1f, 5f);
        Vector2 realPos = new Vector2(pos.X + randOffset, pos.Y);
        Node2D r = sunScene.Instantiate<Node2D>();
        r.Position = realPos;
        //
        IReword s = r as IReword;
        if (s != null)
        {
            s.Init(rewordName, i, value); // 每个阳光25点
        }
        AddChild(r);
    }
}
