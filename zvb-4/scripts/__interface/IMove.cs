namespace ZVB4.Interface
{
    using Godot;
    using ZVB4.Conf;

    public interface IMove
    {
        bool SetMyPosition(Vector2 pos);
        Vector2 GetMyPosition();
        Vector2 GetBasicDirection();
        void StartMove();
        void PauseMove();
        EnumMoveType GetEnumMoveType();
    }
}
