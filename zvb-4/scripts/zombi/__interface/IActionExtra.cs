namespace ZVB4.Interface
{
    using Godot;
    using ZVB4.Conf;

    public interface IActionExtra
    {
        float HasInitAction();
        bool DoInitAction();

        float HasDieAction();
        bool DoDieAction();
    }
}
