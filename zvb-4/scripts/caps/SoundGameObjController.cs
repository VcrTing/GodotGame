
using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Tool;
using System.Collections.Generic;

public partial class SoundGameObjController : Node2D
{
    private const int MaxQueueSize = 21;
    private const int MaxConcurrent = 7;

    private Queue<SoundEffectRequest> _soundQueue = new Queue<SoundEffectRequest>(MaxQueueSize);
    private List<AudioStreamPlayer2D> _players = new List<AudioStreamPlayer2D>();
    public static SoundGameObjController Instance { get; private set; }


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

    private void TryPlayNext()
    {
        foreach (var player in _players)
        {
            if (!player.Playing && _soundQueue.Count > 0)
            {
                var req = _soundQueue.Dequeue();
                var stream = GD.Load<AudioStream>(req.Path);
                if (stream != null)
                {
                    player.Stream = stream;
                    player.VolumeDb = LinearToDb(req.Volume);
                    // Simulate pan by adjusting the X position relative to req.Position
                    // var panOffset = Mathf.Clamp(req.Pan, -1f, 1f) * 100f; // 100 pixels left/right
                    // player.GlobalPosition = req.Position + new Vector2(panOffset, 0);
                    player.Play();
                }
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
    public static void PlayObjHitSound(EnumWhatYouObj whatYouObj, Vector2 position)
    {
        int obji = (int)whatYouObj;
        int c = obji == 1 ? 8 : 4;
        string folderPath = "Hit/" + obji;
        string soundName = "hit_" + obji;
        string path = SoundTool.GetRandomSoundPath(folderPath, soundName, c);
        float pan = SoundTool.CalcPanByX(position.X);
        // GD.Print($"播放受击音效: {path} at {position}, pan={pan}");
        Instance?.EnqueueSound(path, 1f, pan, Vector2.Zero);
    }

    // 示例调用
    public static void PlayBeHurtFx(EnumObjType objType, int damage, EnumHurts enumHurts, EnumWhatYouObj whatYouObj, Vector2 position)
    {
        switch (objType)
        {
            case EnumObjType.Zombie:
                // GD.Print("播放被僵尸攻击受伤音效");
                PlayObjHitSound(whatYouObj, position);
                // Instance?.EnqueueSound("res://audio/zombie_hurt.wav", 1f, 1f, position);
                break;
            case EnumObjType.Plans:
                // GD.Print("播放被植物攻击受伤音效");
                if (enumHurts == EnumHurts.Boom)
                {
                    // GD.Print("NO 播放被植物爆炸攻击受伤音效");
                }
                else
                {
                    PlayObjHitSound(whatYouObj, position);
                    // Instance?.EnqueueSound("res://audio/plant_hurt.wav", 1f, 1f, position);
                }
                break;
            case EnumObjType.Stone:
                GD.Print("播放被Stone攻击受伤音效");
                // Instance?.EnqueueSound("res://audio/stone_hurt.wav", 1f, 1f, position);
                break;
            default:
                GD.Print("播放默认受伤音效");
                break;
        }
    }
}
