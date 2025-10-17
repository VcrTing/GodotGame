

using Godot;

public struct ShootBulletRequest
{
    public Vector2 direction;
    public Vector2 startPosition;
    public ShootBulletRequest(Vector2 direction, Vector2 startPosition)
    {
        this.direction = direction;
        this.startPosition = startPosition;
    }
}