using Godot;

namespace ZVB4.Interface
{
    public interface IShooter
    {
        void AttackAtPosition(Vector2 startPosition, Vector2 direction);
        void RotateToDirection(Vector2 direction);

        void ChangeShooter(string shooterName);
    }
}
