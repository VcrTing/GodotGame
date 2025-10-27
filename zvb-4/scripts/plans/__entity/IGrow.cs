using ZVB4.Conf;

namespace ZVB4.Interface
{
    public interface IGrow
    {
        void DoGrowingStart();
        void DoGrowing(double delta);
        void DoGrowingEnd();

        void FinishedGrowNow();

        void SetGrowTime(float t);

        bool IsGrowed();
    }
}
