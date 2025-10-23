using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansXianRenQiu : Node2D, IObj, IBeHurt, IWorking, IAttack
{
    AnimatedSprite2D view;
    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.XianRenQiu;
    public string GetObjName() => objName;
    public bool Init(string name = null) => true;
    Area2D attackArea;
    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        damage = BulletConstants.GetPlansDamage(objName);
        InitAttackSpeed = BulletConstants.GetPlansAttackSpeed(objName);
        __attackLimit = InitAttackSpeed;
        CloseAttack();
    }
    bool canBeHurt = true;
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!canBeHurt) return false;
        IHealth ih = view as IHealth;
        if (ih == null) return false;
        int yichu = ih.CostHealth(objType, damage, enumHurts);
        if (yichu > 0)
        {
            Die(objType, yichu, enumHurts); return false;
        }
        return true;
    }
    float outroTime = 0.5f;
    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        Node2D beHurtArea = GetNodeOrNull<Node2D>(NameConstants.BeHurtArea);
        if (beHurtArea != null) beHurtArea.QueueFree();
        await ToSignal(GetTree().CreateTimer(outroTime), "timeout");
        return Die();
    }
    public bool Die() {
        GeZi gz = GetParent() as GeZi;
        if (gz != null) gz.UnLockGezi(this);
        QueueFree();
        return true;
    }

    float minScale = GameContants.MinScale;
    float maxScale = GameContants.MaxScale;
    bool isWorking = false;
    public void SetWorkingMode(bool working)
    {
        isWorking = working;
        if (working)
        {
            canBeHurt = true;
            StartAttack();
        }
        else
        {
            CloseAttack();
            canBeHurt = false;
        }
    }

    public bool IsWorking() => isWorking;

    public bool CanAttack() => isWorking;

    float damage = 0f;
    public int GetDamage() => (int)damage;
    public int GetDamageExtra() => 0;

    void StartAttack()
    {
        attackArea.Monitoring = true;
        attackArea.Monitorable = true;
    }
    void CloseAttack()
    {
        attackArea.Monitoring = false;
        attackArea.Monitorable = false;
    }

    float InitAttackSpeed = 1f;
    float __attackLimit = 1f;
    float __attackTime = 0f;
    float __stayedTime = 0f;
    public override void _Process(double delta)
    {
        ViewTool.View3In1(this, minScale, maxScale);

        if (isWorking)
        {
            __attackTime += (float)delta;
            if (__attackTime >= __attackLimit)
            {
                StartAttack();
                __stayedTime += (float)delta;
                if (__stayedTime > 0.2f)
                {
                    __stayedTime = 0f;
                    __attackTime = 0f;
                }
            }
            else
            {
                CloseAttack();
            }
        }
    }

    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts) => ObjTool.DoPlansCure(view as IHealth, objType, cureAmount);

}
