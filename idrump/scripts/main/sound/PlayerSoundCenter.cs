using Godot;
using System;

public partial class PlayerSoundCenter : Node2D
{
	private static readonly string[] DumpMusicPaths = new string[]
	{
		"res://musics/dump/1.mp3",
		"res://musics/dump/2.mp3",
		"res://musics/dump/3.mp3",
		"res://musics/dump/4.mp3",
		"res://musics/dump/5.mp3"
	};

	private Random random = new Random();

	public void WhenPlayerDump()
	{
		int idx = random.Next(DumpMusicPaths.Length);
		string path = DumpMusicPaths[idx];
		var stream = GD.Load<AudioStream>(path);
		if (stream == null)
		{
			GD.PrintErr($"Failed to load music: {path}");
			return;
		}
		var player = new AudioStreamPlayer();
		player.Stream = stream;
		AddChild(player);
		player.Finished += () => player.QueueFree();
		player.Play();
	}
}
