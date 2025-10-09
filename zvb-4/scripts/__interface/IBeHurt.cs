using System.Threading.Tasks;
using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IBeHurt
    {

        bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts);

        Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts);
    }
}
