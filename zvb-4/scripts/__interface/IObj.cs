using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IObj
    {
        EnumObjType GetEnumObjType();
        string GetObjName();

        bool Init(string name = null);

        bool Die();
    }
}
