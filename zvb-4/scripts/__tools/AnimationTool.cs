
using Godot;
using ZVB4.Conf;

public static class AnimationTool
{
    public static void DoAniDefault(AnimatedSprite2D view)
    {
        view?.Play("default");
    }
    public static void DoAniWalk(AnimatedSprite2D view)
    {
        view?.Play("walk");
    }
    public static float DoAniWalkHp(AnimatedSprite2D view, int health, int healthInit)
    {
        if (view == null) return 0f;
        float rate = (float)health / (float)healthInit;
        if (rate <= 0.0f)
        {
            view?.Play("outro");
            return 0.2f;
        }
        else if (rate <= 0.3f)
        {
            view?.Play("walk30");
        }
        else if (rate <= 0.6f)
        {
            view?.Play("walk60");
        }
        else
        {
            view?.Play("walk");
        }
        return 0f;
    }
    public static float DoAniExtraLiveHp(AnimatedSprite2D view, int health, int healthInit)
    {
        if (view == null) return 0f;
        float rate = (float)health / (float)healthInit;
        if (rate <= 0.0f)
        {
            view?.Play("outro");
            return 0.2f;
        }
        else if (rate <= 0.3f)
        {
            view?.Play("live30");
        }
        else if (rate <= 0.6f)
        {
            view?.Play("live60");
        }
        else
        {
            view?.Play("live");
        }
        return 0f;
    }
    public static void DoAniAttack(AnimatedSprite2D view)
    {
        view?.Play("attack");
    }
    public static void DoAniBeHurt(AnimatedSprite2D view)
    {
        view?.Play("behurt");
    }
    public static void DoAniOutro(AnimatedSprite2D view)
    {
        view?.Play("outro");
    }
    public static void DoAniIntro(AnimatedSprite2D view)
    {
        view?.Play("intro");
    }
    public static void DoAniChanging(AnimatedSprite2D view)
    {
        view?.Play("changing");
    }
    public static void DoAniDieForBoom(AnimatedSprite2D view)
    {
        view?.Play("dieforboom");
    }
    public static void DoAniDie(AnimatedSprite2D view)
    {
        view?.Play("die");
    }
}