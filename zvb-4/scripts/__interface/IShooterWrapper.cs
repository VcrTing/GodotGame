using Godot;

namespace ZVB4.Interface
{
    public interface IShooterWrapper
    {
        bool AttackAtPosition(Vector2 startPosition, Vector2 direction);
        void RotateToDirection(Vector2 direction);

        void ChangeShooter(string shooterName);

        void Attack(Vector2 attackPos, bool isFirstAttack, Vector2? startPos = null);
        void ReleaseAttack(bool mustEmpty = false);
    }
}
