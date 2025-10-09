using Godot;
using System;
using System.Collections.Generic;
using ZVB4.Conf;

public partial class EnmyCenter : Node2D
{
    private float _timer = 0f;
    private float _nextInterval = 0f;

    public static EnmyCenter Instance { get; set; }

    private Dictionary<string, int> paoxiaoDict = new Dictionary<string, int>
    {
        { "s_long", 4 },
        { "s_middle", 3 },
        { "s_short", 2 }
    };

    private int cccCount = 1; // 0=暂停，1~5=执行多少次CCC，最多5次

    public bool PaoxiaoEnabled { get; set; } = true;

    private float minInterval = 5.0f;
    private float maxInterval = 8.0f;

    public override void _Ready()
    {
        Instance = this;
        SetNextInterval();
    }

    float aloneSoundTimer = 0f;
    float aloneSoundInterval = 1f; // 每10秒检查一次
    public override void _Process(double delta)
    {
        _timer += (float)delta;
        if (_timer >= _nextInterval)
        {
            _timer = 0f;
            SetNextInterval();
            if (!PaoxiaoEnabled) return;
            PlayZombiPaoxiao();
        }

        aloneSoundTimer += (float)delta;
        if (aloneSoundTimer >= aloneSoundInterval)
        {
            aloneSoundTimer = 0f;
            count -= 1;
            if (count < 0) count = 0;
        }
    }

    void PlayZombiPaoxiao() {
        if (paoxiaoDict.Count > 0)
        {
            var keys = new List<string>(paoxiaoDict.Keys);
            int randIdx = GD.RandRange(0, keys.Count - 1);
            string name = keys[randIdx];
            int num = paoxiaoDict[name];
            SoundGameObjController.Instance?.PlayFxRandomPos("Zombi/paoxiao", name, num);
        }
    }

    private void SetNextInterval()
    {
        _nextInterval = (float)(GD.RandRange(minInterval, maxInterval));
    }

    public void SetPaoxiaoEnabled(bool enabled)
    {
        PaoxiaoEnabled = enabled;
        if (PaoxiaoEnabled)
        {
            PlayZombiPaoxiao();
        }
    }
    int count = 0;
    public void PlayAlonePaoxiao(string name, Vector2 pos)
    {
        count += 1;
        if (name == EnmyTypeConstans.ZombiM) 
        {
            SoundGameObjController.Instance?.PlayFx("Zombi/kid", name, 4, pos);
        }
    }
}
