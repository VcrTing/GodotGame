

using System;
using System.Text.Json;
using Godot;
using ZVB4.Entity;

public partial class EndLessGameDataManager : Node
{
    private const string SaveFileName = "user://endlessgamedata.json";
    public static EndLessGameDataManager Instance { get; private set; }

    private EntityGameRunnerData _gameRunnerData = null;

    public override void _Ready()
    {
        Instance = this;
        LoadGameRunnerData();
    }

    public EntityGameRunnerData GetGameRunnerData()
    {
        if (_gameRunnerData == null)
        {
            LoadGameRunnerData();
        }
        return _gameRunnerData;
    }

    public void SaveGameRunnerData()
    {
        try
        {
            string json = JsonSerializer.Serialize(_gameRunnerData);
            using var file = Godot.FileAccess.Open(SaveFileName, Godot.FileAccess.ModeFlags.Write);
            file.StoreString(json);
            file.Close();
        }
        catch (Exception e)
        {
            GD.PrintErr($"Save failed: {e.Message}");
        }
    }

    public void LoadGameRunnerData()
    {
        try
        {
            if (!Godot.FileAccess.FileExists(SaveFileName))
            {
                _gameRunnerData = new EntityGameRunnerData();
                SaveGameRunnerData();
            }
            if (Godot.FileAccess.FileExists(SaveFileName))
            {
                using var file = Godot.FileAccess.Open(SaveFileName, Godot.FileAccess.ModeFlags.Read);
                string json = file.GetAsText();
                file.Close();
                _gameRunnerData = JsonSerializer.Deserialize<EntityGameRunnerData>(json) ?? new EntityGameRunnerData();
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Load failed: {e.Message}");
            _gameRunnerData = new EntityGameRunnerData();
            SaveGameRunnerData();
        }
    }
}