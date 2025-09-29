

using Godot;

public struct SoundEffectRequest
{
    public string Path;
    public float Volume;
    public float Pan; // -1左 0中 1右
    public Vector2 Position;
    public SoundEffectRequest(string path, float volume, float pan, Vector2 pos)
    {
        Path = path;
        Volume = volume;
        Pan = pan;
        Position = pos;
    }
}