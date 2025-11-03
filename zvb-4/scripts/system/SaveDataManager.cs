
using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ZVB4.Conf;
using ZVB4.Entity;

public partial class SaveDataManager : Node
{
    private const string SaveFileName = "user://playerdata.json";
    public static SaveDataManager Instance { get; private set; }

    private EntityPlayerData _playerData = null;

    public override void _Ready()
    {
        Instance = this;
        LoadPlayerData();
        
        // ResetData();
    }

    public void ResetData() {
        if (_playerData == null) { LoadPlayerData(); }
        _playerData.Money = 3000;
        _playerData.PlansUnLock = "_" + PlansConstants.Pea + "_" + "_" + PlansConstants.SunFlower + "_";
        _playerData.ShooterUnLimit = "_" + PlansConstants.Pea + "_";
        _playerData.ShooterBaseLast = PlansConstants.Pea;
        SavePlayerData();
        // GD.Print("REST GAME DATA");
    }

    public EntityPlayerData GetPlayerData()
    {
        if (_playerData == null) { LoadPlayerData(); }
        return _playerData;
    }

    public void SavePlayerData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_playerData);
            using var file = Godot.FileAccess.Open(SaveFileName, Godot.FileAccess.ModeFlags.Write);
            file.StoreString(json);
            file.Close();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Save failed: {e.Message}");
        }
    }

    public void LoadPlayerData()
    {
        try
        {
            if (!Godot.FileAccess.FileExists(SaveFileName))
            {
                _playerData = new EntityPlayerData();
                SavePlayerData();
            }
            if (Godot.FileAccess.FileExists(SaveFileName))
            {
                using var file = Godot.FileAccess.Open(SaveFileName, Godot.FileAccess.ModeFlags.Read);
                string json = file.GetAsText();
                file.Close();
                _playerData = JsonSerializer.Deserialize<EntityPlayerData>(json) ?? new EntityPlayerData();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Load failed: {e.Message}");
            _playerData = new EntityPlayerData();
            SavePlayerData();
        }
    }

    public void SetPlayerShooter(string shooterName)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            _playerData.ShooterNow = shooterName;
            SavePlayerData();
        }
    }
    public string GetPlayerShooter()
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData == null) return PlansConstants.Pea;
        return _playerData.ShooterNow;
    }
    public string GetLastBaseShooter()
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData == null) {
            return PlansConstants.Pea;
        }
        return _playerData.ShooterBaseLast;
    }

    public void TrySavePlayerShooterBaseLast(string shooterName)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            _playerData.ShooterBaseLast = shooterName;
            // GD.Print("保存基础射手 = " + shooterName);
            SavePlayerData();
        }
    }
    public void SetMoneyAndSave(int value)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            _playerData.Money = value;
            SavePlayerData();
        }
    }
    public void AddMoneyAndSave(int value)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            _playerData.Money += value;
            // GD.Print("AddMoneyAndSave: " + value + " total: " + _playerData.Money);
            SavePlayerData();
        }
    }
    public int GetMoney()
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            return _playerData.Money;
        }
        return 0;
    }
    // 射手限制
    public bool IsShooterUnLimit(string n)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            string p = _playerData.ShooterUnLimit;
            if (p.Contains("_" + n + "_"))
            {
                return true;
            }
        }
        return false;
    }
    public void UnLockShooterUnLimit(string n) 
    {
        bool has = IsShooterUnLimit(n);
        if (!has)
        {
            string p = _playerData.ShooterUnLimit;
            p += ("_" + n + "_");
            _playerData.ShooterUnLimit = p;
            SavePlayerData();
        }
    }
    public List<string> GetShooterUnLimitList()
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        { return CommonTool.SplitStringToList(_playerData.ShooterUnLimit); }
        return null;
    }
    // 植物
    public bool HasPlans(string n)
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null)
        {
            string p = _playerData.PlansUnLock;
            if (p.Contains("_" + n + "_")) { return true; }
        }
        return false;
    }
    public void UnlockPlans(string n)
    {
        bool has = HasPlans(n);
        if (!has)
        {
            string p = _playerData.PlansUnLock;
            p += ("_" + n + "_");
            _playerData.PlansUnLock = p;
            SavePlayerData();
        }
    }
    public List<string> GetPlansUnLockList()
    {
        if (_playerData == null) { LoadPlayerData(); }
        if (_playerData != null) { return CommonTool.SplitStringToList(_playerData.PlansUnLock); }
        return null;
    }
}
