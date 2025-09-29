using Godot;
using System;
using System.Collections.Generic;

public partial class EnmyCenter : Node2D
{
    private float _timer = 0f;
    private float _nextInterval = 0f;
    private Random _rand = new Random();

    private Dictionary<string, int> paoxiaoDict = new Dictionary<string, int>
    {
        { "s_long", 4 },
        { "s_middle", 3 },
        { "s_short", 2 }
    };

    private int cccCount = 1; // 0=暂停，1~5=执行多少次CCC，最多5次

    public bool PaoxiaoEnabled { get; set; } = true;

    private float minInterval = 3.0f;
    private float maxInterval = 5.0f;

    public override void _Ready()
    {
        SetNextInterval();
    }

    public override void _Process(double delta)
    {
        _timer += (float)delta;
        if (_timer >= _nextInterval)
        {
            _timer = 0f;
            SetNextInterval();
            if (!PaoxiaoEnabled) return;
            // 加入某个声音（示例："fx", "enmy", 3）
            // CCC
            PlayZombiPaoxiao();
        }
    }

    void PlayZombiPaoxiao() {
        if (paoxiaoDict.Count > 0)
        {
            var keys = new List<string>(paoxiaoDict.Keys);
            int randIdx = GD.RandRange(0, keys.Count - 1);
            string k = keys[randIdx];
            int v = paoxiaoDict[k];
            SoundFxController.Instance?.PlayFx("Zombi/paoxiao", k, v);
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
}
