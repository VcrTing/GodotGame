using Godot;
using System;
using System.Collections.Generic;

public partial class ThingsMoney : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is PlayerBody player)
		{
			AddGold(player);
		}
	}

	private float _disappearTimer = -1f;
    bool _isDisappearing = false;

	private void AddGold(PlayerBody player)
    {
        if (_isDisappearing) return;
        // TODO: 实现加金币逻辑，例如：player.AddGold(1);
        GD.Print("金币+1");
        _isDisappearing = true;
        //
        Systems.Instance.AddMoney(1);
        // 播放音效
        PlaySound();
        // 吃掉后变透明，2秒后消失
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0f);
        SetProcess(true);
        _disappearTimer = 2f;
    }

    List<String> SoundList = new List<String>()
    {
        "res://musics/eat/1.mp3",
        "res://musics/eat/2.mp3",
        "res://musics/eat/3.mp3",
        // 可以添加更多音效路径
    };

    void PlaySound()
    {
        if (SoundList.Count == 0) return;
        var rand = new System.Random();
        int idx = rand.Next(SoundList.Count);
        SoundEffectPlayer.Play(SoundList[idx], 0.5f);
    }

	public override void _Process(double delta)
    {
        if (_disappearTimer > 0f)
        {
            _disappearTimer -= (float)delta;
            if (_disappearTimer <= 0f)
            {
                QueueFree();
            }
        }
    }
}
