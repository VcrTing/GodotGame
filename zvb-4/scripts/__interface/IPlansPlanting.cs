using Godot;
namespace ZVB4.Interface
{
    public interface IPlansPlanting
    {
        bool CanZhongZhiPlans(Vector2 pos, string planName);
        bool ZhongZhiPlans(Vector2 pos, string planName);
    }
}
