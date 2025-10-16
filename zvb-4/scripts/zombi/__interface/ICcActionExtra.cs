namespace ZVB4.Interface
{
    using Godot;
    using ZVB4.Conf;

    public interface ICcActionExtra: IActionExtra
    {
        float HasChangeAction();
        bool StartChangeAction();
        bool EndChangeAction();
    }
}
