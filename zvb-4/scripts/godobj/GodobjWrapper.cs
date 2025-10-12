using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;

using ZVB4.Interface;
using ZVB4.Tool;

public partial class GodobjWrapper : Node2D, IMove, IBeHurt, IObj, IAttack
{
    bool canMove = true;

    // 移动控制
    public bool SetMyPosition(Vector2 pos)
    {
        if (!canMove) return false;
        Position = pos;
        return true;
    }
    public void StartMove()
    {
        canMove = true;
    }
    public void PauseMove()
    {
        canMove = false;
    }

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;

    public bool Init(string name)
    {
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        damage = ObjTool.GetObjDamage(objType, objName);
        damageExtra = ObjTool.GetObjDamageExtra(objType, objName);
        // GD.Print($"GodobjWrapper Init: {objName}, damage: {damage}, damageExtra: {damageExtra}");
        AdjustView();
        return true;
    }
    public override void _Ready()
    {
        Init(objName);
    }
    public bool AdjustView()
    {
        ViewTool.View3In1(this, minScale, maxScale);
        return true;
    }
    public override void _Process(double delta)
    {
        AdjustView();
    }


    [Export]
    public EnumObjType objType = EnumObjType.Zombie;
    public EnumObjType GetEnumObjType() => objType;
    public void SetEnumObjType(EnumObjType type)
    {
        objType = type;
    }
    [Export]
    public string objName = EnmyTypeConstans.ZombiS;
    public string GetObjName() => objName;
    public void SetObjName(string name)
    {
        objName = name;
    }

    [Export]
    public int damage = 0;
    public int GetDamage() => damage;
    [Export]
    public int damageExtra = 0;
    public int GetDamageExtra() => damageExtra;
    [Export]
    public EnumMoveType moveType = EnumMoveType.LineWalk;
    public EnumMoveType GetEnumMoveType() => moveType;


    // 受伤控制（IHurt接口实现）
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!canBeHurt) return false;
        bool died = ObjTool.RunningBeHurt(this, objType, damage, enumHurts);
        // GD.Print($"{Name} 受伤，掉血 {damage}. ");
        if (died)
        {
            _ = Die(objType, damage, enumHurts);
        }
        return died;
    }
    public bool BeCure(int cure)
    {
        GD.Print($"{Name} 治疗，回血 {cure}. ");
        return false;
    }
    bool canBeHurt = true;
    public void StopBeHurt() => canBeHurt = false;
    public void StartBeHurt() => canBeHurt = true;

    public async Task<bool> Die(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        // 生成奖励
        IWhenDie whenDie = this.GetNodeOrNull<IWhenDie>(NameConstants.WorkingDumpReword);
        whenDie?.WorkingWhenDie(objName);
        ObjTool.RunningDie(this, objType, damage, enumHurts);
        await ToSignal(GetTree().CreateTimer(AnimationConstants.GetDieAniTime(this)), "timeout");
        Die();
        return true;
    }


    public bool CanDie()
    {
        return true;
    }

    public bool Die()
    {
        //
        QueueFree();
        return true;
    }

    public Vector2 GetMyPosition()
    {
        throw new NotImplementedException();
    }

    public bool CanAttack()
    {
        throw new NotImplementedException();
    }
}
