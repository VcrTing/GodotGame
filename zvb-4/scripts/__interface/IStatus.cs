namespace ZVB4.Interface
{
    using Godot;
    using ZVB4.Conf;

    public interface IStatus
    {
        bool DoFreeze(float time);
        bool DoCold(float time);

        bool ReleaseFreeze();
        bool ReleaseCold();

        float GetMoveSpeedScale();
        float GetAttackSpeedScale();
        float GetAnimationSpeedScale();

    }
}
