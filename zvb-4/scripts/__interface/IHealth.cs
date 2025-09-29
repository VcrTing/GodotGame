using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IHealth
    {
        bool IsDie();
        bool UpHealth(int heal);
        int CostHealth(EnumObjType objType, int damage, EnumHurts enumHurts);
    }
}
