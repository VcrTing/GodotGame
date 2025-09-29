using System.Threading.Tasks;
using Godot;
using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IBulletBase
    {
        void SetDirection(Vector2 direction);
        Vector2 GetDirection();
        void FlipXDirection();
        EnumHurts GetHurtType();
    }
}
