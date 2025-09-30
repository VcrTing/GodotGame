
using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Tool;

public partial class SoundUiController : Node2D
{
    private AudioStreamPlayer2D _player;
    private Queue<string> _soundQueue = new Queue<string>();
    private bool _isCooldown = false;

    float limit = 0.05f;

    public static SoundUiController Instance { get; private set; }

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

    // sure 
    public void Sure()
    {
        EnqueueSound("Ui/btn_sure_down", "sure", 4);
    }

    // back
    public void Back()
    {
        EnqueueSound("Ui", "back", 2);
    }

    // error
    public void Error()
    {
        EnqueueSound("Ui", "error", 3);
    }

    // win
    public void Win()
    {
        EnqueueSound("Ui/win", "win_s", 4);
    }
    // die
    public void Die()
    {
        EnqueueSound("Ui/losegame", "game_over_ready", 4);
    }
}
