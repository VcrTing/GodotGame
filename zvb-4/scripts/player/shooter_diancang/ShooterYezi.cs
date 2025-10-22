using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class ShooterYezi : Node2D, IObj, IPao
{
    AudioStreamPlayer2D _rotaionSoundPlayer;
    public override void _Ready()
    {
        _rotaionSoundPlayer = GetNodeOrNull<AudioStreamPlayer2D>("RotationSoundPlayer");
        if (_rotaionSoundPlayer != null)
        {
            _rotaionSoundPlayer.Play();
        }
    }

    [Export]
    public EnumObjType ObjType { get; set; } = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => ObjType;
    [Export]
    public string ObjName { get; set; } = PlansConstants.Yezi;
    public string GetObjName() => ObjName;
    bool init = false;
    public bool Init(string name = null)
    {
        ObjName = name ?? PlansConstants.Yezi;
        LoadBullet();
        DoInitEffect(GlobalPosition);
        init = true;
        return true;
    }
    float deg = 8f;
    public List<Vector2> ComputedAttackDirections(Vector2 startPosition, Vector2 direction)
    {
        List<Vector2> ds = new List<Vector2>();
        ds.Add(direction);
        //
        return ds;
    }
    void __Shoot(PackedScene bulletScene, Vector2 startPosition, Vector2 direction)
    {
        var bullet = bulletScene.Instantiate<Node2D>();
        bullet.Position = startPosition;
        // 调整运动方向
        if (bullet is IBulletBase bulletBase)
        {
            bulletBase.SetDirection(direction.Normalized());
        }
        GetTree().CurrentScene.AddChild(bullet);
    }
    public void ShootBullet(Vector2 startPosition, Vector2 direction)
    {
        // 正式攻击
        var bulletScene = GD.Load<PackedScene>(BulletScenePath);
        if (bulletScene != null)
        {
            List<Vector2> directions = ComputedAttackDirections(startPosition, direction);
            foreach (var dir in directions)
            {
                __Shoot(bulletScene, startPosition, dir);
            }
            // 播放音效
            DoFireEffect(startPosition);
        }
    }
    // 获取射手对应的子弹实例
    [Export]
    public string BulletScenePath { get; set; } = string.Empty;
    public void LoadBullet()
    {
        BulletScenePath = BulletConstants.GetBullet(ObjName);
    }
    public void DoFireEffect(Vector2 position)
    {
        SoundPlayerController.Instance?.EnqueueSound("Bullets/" + ObjName, ObjName, 6);
    }
    public void DoInitEffect(Vector2 position)
    {
        SoundPlayerController.Instance?.EnqueueSound("Bullets/" + ObjName + "/Load", ObjName + "_load", 4);
    }
    public void DoFireLoadEffect(Vector2 position)
    {
        SoundFxController.Instance.PlayFx("Bullets/" + ObjName + "/Load", ObjName + "_load", 4, position);
    }
    public bool Die()
    {
        QueueFree();
        return true;
    }

    bool rotating = false;
    public void OnRotingStart(Vector2 dirTarget)
    {
        rotating = true;
    }

    public void OnRotingEnd(Vector2 dirNow)
    {
        rotating = false;
    }

    public void OnFireStart(Vector2 direction)
    {
    }

    float _soundTargetVolume = 0f;
    float _soundFadeSpeed = 0f;
    float _soundMaxVolume = 1f;
    float _soundMinVolume = 0f;

    public override void _Process(double delta)
    {
        RunningForRotatingSound(delta);
    }

    void RunningForRotatingSound(double delta)
    {
        // KKK
        if (_rotaionSoundPlayer == null) return;
        if (rotating)
        {
            _soundTargetVolume = _soundMaxVolume;
            _soundFadeSpeed = (float)(delta / 0.05f); // 0.05秒到最大
            if (!_rotaionSoundPlayer.Playing)
                _rotaionSoundPlayer.Play();
        }
        else
        {
            _soundTargetVolume = _soundMinVolume;
            _soundFadeSpeed = (float)(delta / 0.05f); // 0.05秒到最小
        }
        // 音量渐变
        float curVol = _rotaionSoundPlayer.VolumeDb;
        float targetDb = Mathf.Lerp(curVol, LinearToDb(_soundTargetVolume), _soundFadeSpeed);
        _rotaionSoundPlayer.VolumeDb = targetDb;
        // 停止播放
        if (!rotating && _rotaionSoundPlayer.VolumeDb <= LinearToDb(0.01f))
        {
            _rotaionSoundPlayer.Stop();
            _rotaionSoundPlayer.VolumeDb = LinearToDb(0f);
        }
    }
    // 工具：线性音量转分贝
    float LinearToDb(float linear)
    {
        if (linear <= 0f) return -80f;
        return 20f * (float)Math.Log10(linear);
    }
}
