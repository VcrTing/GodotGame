using Godot;
using System;
using ZVB4.Tool;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class SoundOneshotController : Node2D
{
    public static SoundOneshotController Instance { get; private set; }
	private AudioStreamPlayer2D _player;
	private AudioStreamPlayer2D _player2;
	private Queue<SoundEffectRequest> _clipQueue = new Queue<SoundEffectRequest>();
	private float _lastEnqueueTime = -100f;
	private int _playingCount = 0;
	private Dictionary<string, float> _soundKeyDict = new Dictionary<string, float>();

	public override void _Ready()
    {
        Instance = this;
        // 初始化两个播放器，交替使用
		_player = GetNodeOrNull<AudioStreamPlayer2D>(NameConstants.AudioStreamPlayer2D);
		if (_player == null)
		{
			_player = new AudioStreamPlayer2D();
			AddChild(_player);
		}
		_player.Finished += OnPlayerFinished;
		_player.Bus = SoundTool.SoundBusUx;

		_player2 = new AudioStreamPlayer2D();
		AddChild(_player2);
		_player2.Finished += OnPlayerFinished;
		_player2.Bus = SoundTool.SoundBusUx;
        //
		SetProcess(true);
	}

    // 加载声音切片并设置音量和坐标后放入播放队列
    public void PlayFx(string folderPath, string soundName, int soundIndex, float volume, Vector2 pos, float expiredSeconds = 2f)
    {
        string soundkey = folderPath + soundName;
        float now = Time.GetTicksMsec() / 1000.0f;
        // 检查是否已在dict中
        if (_soundKeyDict.ContainsKey(soundkey))
            return;
        string path = SoundTool.GetRandomSoundPath(folderPath, soundName, soundIndex);
        var stream = GD.Load<AudioStream>(path);
        if (stream == null) return;
        PlayClip(new SoundEffectRequest { Path = path, Pan = SoundTool.CalcPanByX(pos.X), Volume = volume });
        // 加入dict，2秒后过期
        _soundKeyDict[soundkey] = now + expiredSeconds;
    }

    float __t = 0f;
    
	public override void _Process(double delta)
	{
        __t += (float)delta;
        if (__t > 0.5f)
        {
            __t = 0f;
            // 每0.5s清理一次dict中过期的key
            float now = Time.GetTicksMsec() / 1000.0f;
            List<string> expired = new List<string>();
            foreach (var kv in _soundKeyDict)
            {
                if (kv.Value < now)
                    expired.Add(kv.Key);
            }
            foreach (var k in expired)
            {
                _soundKeyDict.Remove(k);
            }
        }
	}
    
    // 播放音效
	public void PlayClip(SoundEffectRequest effectRequest)
	{
		float now = Time.GetTicksMsec() / 1000.0f;
		if (_playingCount >= 2)
			return; // 最多2个并发
		if (_playingCount > 0 && now - _lastEnqueueTime < 0.5f)
			return; // 播放中，0.5s内不再加入
		_clipQueue.Enqueue(effectRequest);
		_lastEnqueueTime = now;
		TryPlayNext();
	}

	private void TryPlayNext()
	{
		if (_clipQueue.Count == 0)
			return;
		if (!_player.Playing)
		{
            SoundEffectRequest param = _clipQueue.Dequeue();
            AudioStream stream = GD.Load<AudioStream>(param.Path);
            if (stream == null) return;
			_player.Stream = stream;
			_player.PanningStrength = Mathf.Abs(param.Pan);
			_player.VolumeDb = Mathf.LinearToDb(param.Volume);
			_player.Play();
			_playingCount++;
		}
		else if (!_player2.Playing)
		{
		    SoundEffectRequest param = _clipQueue.Dequeue();
            AudioStream stream = GD.Load<AudioStream>(param.Path);
            if (stream == null) return;
			_player2.Stream = stream;
			_player.PanningStrength = Mathf.Abs(param.Pan);
			_player2.VolumeDb = Mathf.LinearToDb(param.Volume);
			_player2.Play();
			_playingCount++;
		}
	}

	private void OnPlayerFinished()
	{
		_playingCount = Math.Max(0, _playingCount - 1);
		TryPlayNext();
	}
}
