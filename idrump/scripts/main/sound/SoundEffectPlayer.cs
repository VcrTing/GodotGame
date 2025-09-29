using Godot;
using System;
using System.Collections.Generic;

public partial class SoundEffectPlayer : Node2D
{
    public static SoundEffectPlayer Instance { get; private set; }
    private List<AudioStreamPlayer2D> _playerPool = new List<AudioStreamPlayer2D>();

    public override void _Ready()
    {
        Instance = this;
        // 初始化播放器池
        for (int i = 0; i < 8; i++)
        {
            var player = new AudioStreamPlayer2D();
            AddChild(player);
            _playerPool.Add(player);
        }
    }
    // 获取可用播放器
    private AudioStreamPlayer2D GetAvailablePlayer()
    {
        foreach (var player in _playerPool)
        {
            if (!player.Playing)
                return player;
        }
        return null;
    }

    // 线性音量转分贝
    private static float LinearToDb(float linear)
    {
        if (linear <= 0f)
            return -80f;
        return 20f * (float)Math.Log10(linear);
    }
    /// <summary>
    /// 静态方法：直接播放指定音频资源路径
    /// </summary>
    public static void Play(string path, float volume = 0.7f, float pitch = 1.0f)
    {
        Instance?.PlaySoundInstance(path, volume, pitch);
    }

    // 实例方法：播放音频
    private void PlaySoundInstance(string path, float volume = 0.7f, float pitch = 1.0f)
    {
        var player = GetAvailablePlayer();
        if (player == null)
        {
            GD.PrintErr($"音效池已满，无法播放: {path}");
            return;
        }
        var stream = GD.Load<AudioStream>(path);
        if (stream == null)
        {
            GD.PrintErr($"音效加载失败: {path}");
            return;
        }
        player.Stream = stream;
        player.PitchScale = pitch;
        player.VolumeDb = LinearToDb(volume);
        player.Play();
    }
    
}