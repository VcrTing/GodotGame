using System.Threading.Tasks;
using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IBeHurt
    {
        bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts);

        bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts);

        Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts);
    }
}
