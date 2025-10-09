using Godot;

namespace ZVB4.Interface
{
    public interface IShooter
    {
        void ShootBullet(Vector2 startPosition, Vector2 direction);
        void LoadBullet();

        void DoFireEffect(Vector2 position);
        void DoInitEffect(Vector2 position);
        void DoFireLoadEffect(Vector2 position);
    }
}
