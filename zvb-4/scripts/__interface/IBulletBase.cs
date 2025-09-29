using System.Threading.Tasks;

namespace ZVB4.Interface
{
    public interface IBulletBase
    {
        void SetDirection(Godot.Vector2 direction);
        void FlipXDirection();
    }
}
