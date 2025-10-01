
using Godot;
using System;
using System.IO;
using System.Text.Json;
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
    }

    public EntityPlayerData GetPlayerData()
    {
        if (_playerData == null)
        {
            LoadPlayerData();
        }
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

    public void SetPlayerShooter(string shooterName)
    {
        if (_playerData == null)
        {
            LoadPlayerData();
        }
        if (_playerData != null)
        {
            _playerData.ShooterNow = shooterName;
            SavePlayerData();
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

    public void SetMoneyAndSave(int value)
    {
        if (_playerData == null)
        {
            LoadPlayerData();
        }
        if (_playerData != null)
        {
            _playerData.Money = value;
            SavePlayerData();
        }
    }
}
