
using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Tool;
using System.Collections.Generic;

public partial class SoundFxController : Node2D
{

    private const int MaxQueueSize = 7;
    private const int MaxConcurrent = 7;

    private Queue<SoundEffectRequest> _soundQueue = new Queue<SoundEffectRequest>(MaxQueueSize);
    private List<AudioStreamPlayer2D> _players = new List<AudioStreamPlayer2D>();
    public static SoundFxController Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        for (int i = 0; i < MaxConcurrent; i++)
        {
            var player = new AudioStreamPlayer2D();
            player.Name = $"Player_{i}";
            player.Autoplay = false;
            player.Finished += OnPlayerFinished;
            AddChild(player);
            _players.Add(player);
        }
    }

    public void EnqueueSound(string path, float volume = 1f, float pan = 0f, Vector2? pos = null)
    {
        if (_soundQueue.Count >= MaxQueueSize)
            return;
        _soundQueue.Enqueue(new SoundEffectRequest(path, volume, pan, pos ?? Vector2.Zero));
        TryPlayNext();
    }

    static int n = 48;
    void SetLocation(Vector2 pos)
    {
        float x = pos.X;
        if (x > -n && x < 0)
        {
            x = -n;
        }
        if (x < n && x > 0)
        {
            x = n;
        }
        Position = new Vector2(x, 0);
    }

    private void TryPlayNext()
    {
        foreach (var player in _players)
        {
            try
            {
                if (!player.Playing && _soundQueue.Count > 0)
                {
                    var req = _soundQueue.Dequeue();
                    // GD.Print("sound fx working " + req.Path);
                    // 检查文件是否存在
                    if (!FileAccess.FileExists(req.Path)) {
                        // GD.Print("sound fx not exists.");
                        // continue;
                    }
                    // GD.Print("sound fx working " + req.Path);
                    var stream = GD.Load<AudioStream>(req.Path);
                    if (stream != null)
                    {
                        SetLocation(req.Position);
                        player.Stream = stream;
                        player.VolumeDb = LinearToDb(req.Volume);
                        player.PanningStrength = Mathf.Abs(req.Pan);
                        // Simulate pan by adjusting the X position relative to req.Position
                        // var panOffset = Mathf.Clamp(req.Pan, -1f, 1f) * 100f; // 100 pixels left/right
                        // player.GlobalPosition = req.Position + new Vector2(panOffset, 0);
                        player.Play();
                    }
                }
            }
            catch (Exception e)
            {
                GD.PrintErr(e);
            }
        }
    }

    private void OnPlayerFinished()
    {
        // 任何player结束都尝试播放下一个
        TryPlayNext();
    }

    private float LinearToDb(float linear)
    {
        if (linear <= 0f) return -80f;
        return 20f * (float)Math.Log10(linear);
    }

    // 播放受击音效
    public void PlayFx(string folderPath, string soundName, int soundIndex, Vector2 position)
    {
        string path = SoundTool.GetRandomSoundPath(folderPath, soundName, soundIndex);
        float pan = SoundTool.CalcPanByX(position.X);
        Instance?.EnqueueSound(path, 1f, pan, position);
    }

    // 示例调用
    public void PlayFx(string folderPath, string soundName, int soundIndex)
    {
        string path = SoundTool.GetRandomSoundPath(folderPath, soundName, soundIndex);
        Instance?.EnqueueSound(path);
    }

}
