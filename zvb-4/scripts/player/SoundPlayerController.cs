using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Tool;
using System.Collections.Generic;

public partial class SoundPlayerController : Node2D
{
    private AudioStreamPlayer2D _player;
    private Queue<string> _soundQueue = new Queue<string>();
    private bool _isCooldown = false;

    float limit = 0.05f;

    public static SoundPlayerController Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _player = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        if (_player == null)
        {
            _player = new AudioStreamPlayer2D();
            AddChild(_player);
        }
        _player.Finished += OnSoundFinished;
    }

    public void EnqueueSound(string soundPath)
    {
        _soundQueue.Enqueue(soundPath);
        TryPlayNext();
    }

    public void EnqueueSound(string folderPath, string soundName, int soundIndex)
    {
        string path = SoundTool.GetRandomSoundPath(folderPath, soundName, soundIndex);
        EnqueueSound(path);
    }

    private async void TryPlayNext()
    {
        if (_isCooldown || _soundQueue.Count == 0)
            return;
        string path = _soundQueue.Dequeue();
        // 检查文件是否存在
        if (!FileAccess.FileExists(path))
        {
            TryPlayNext();
            return;
        }
        var stream = GD.Load<AudioStream>(path);
        if (stream != null)
        {
            _player.Stream = stream;
            _player.Play();
        }
        else
        {
            GD.PrintErr($"Sound not found: {path}");
        }
        _isCooldown = true;
        await ToSignal(GetTree().CreateTimer(limit), "timeout");
        _isCooldown = false;
        if (_soundQueue.Count > 0)
        {
            TryPlayNext();
        }
    }

    private void OnSoundFinished()
    {
        _isCooldown = false;
        TryPlayNext();
    }
}
