using System.Threading.Tasks;
using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IBeHurt
    {

        bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts);
        bool BeCure(int cure);
        void StopBeHurt();
        void StartBeHurt();

        Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts);
        // void BeHurtEffect(EnumObjType enumAttack, int damage, EnumHurts enumHurts);
    }
}
