using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IHurtBase
    {
        bool TakeDamage(EnumObjType objType, int damage, EnumHurts enumHurts);
    }
}
