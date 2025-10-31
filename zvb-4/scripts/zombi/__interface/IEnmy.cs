
using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IEnmy
    {
        void SetInitScale(float movespeedscale, float behurtscale, float viewscale, float attackspeedscale);
        void SetObjName(string name);

        EnumEnmyStatus GetStatus();
        void SwitchStatus(EnumEnmyStatus status);

        EnumMoveType GetEnumMoveType();

        void SeeTarget(IObj obj);

        void JudgeOpenRedEyeMode(float redeyeratio);
        void StartRedMode();
        void EndRedMode();

        EnumWhatYouObj GetWhatYouObj();
    }
}