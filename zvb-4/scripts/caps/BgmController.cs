using Godot;
using System;
using ZVB4.Conf;

public partial class BgmController : Node2D
{
    private AudioStreamPlayer2D _bgmPlayer;

    [Export]
    public EnumChapter Chapter = EnumChapter.One1; // 章节名称，初始为1，可根据需要修改

    private float _targetVolume = 0f;
    private float _volumeChangeDuration = 0.5f;
    private float _volumeChangeElapsed = 0f;
    private float _startVolume = 0f;
    private float _initVolume = 0f;
    private bool _isVolumeChanging = false;

    public static BgmController Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _bgmPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        if (_bgmPlayer != null)
        {
            _initVolume = _bgmPlayer.VolumeDb;
        }
        PlayChapterBgm((int)Chapter);
        ResumeBgm();
    }

    public void PlayChapterBgm(int chapter)
    {
        // CCC
        string chapterStr = chapter.ToString();
        string firstDigit = chapterStr.Length > 0 ? chapterStr.Substring(0, 1) : "1";
        string path = $"res://musics/BGMs/cap_{firstDigit}.mp3";
        var stream = GD.Load<AudioStream>(path);
        if (stream != null && _bgmPlayer != null)
        {
            _bgmPlayer.Stream = stream;
            // _bgmPlayer.Looping = true;
            _bgmPlayer.Play();
        }
        else
        {
            GD.PrintErr($"BGM not found: {path}");
        }
    }

    public void FadeToVolume(float targetVolume)
    {
        if (_bgmPlayer == null) return;
        _startVolume = _bgmPlayer.VolumeDb;
        _targetVolume = targetVolume;
        _volumeChangeElapsed = 0f;
        _isVolumeChanging = true;
    }

    public void PauseBgm()
    {
        // Godot官方推荐的最小音量为-80dB，接近静音
        FadeToVolume(-80f);
    }

    public void ResumeBgm()
    {
        FadeToVolume(_initVolume);
    }

    public override void _Process(double delta)
    {
        if (_isVolumeChanging && _bgmPlayer != null)
        {
            _volumeChangeElapsed += (float)delta;
            float t = Mathf.Clamp(_volumeChangeElapsed / _volumeChangeDuration, 0, 1);
            float newVolume = Mathf.Lerp(_startVolume, _targetVolume, t);
            _bgmPlayer.VolumeDb = newVolume;
            if (t >= 1f)
            {
                _isVolumeChanging = false;
            }
        }
    }
}
