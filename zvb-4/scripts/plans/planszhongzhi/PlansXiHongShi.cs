using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class PlansXiHongShi : Node2D, IObj, IBeHurt, IWorking, IAttack
{
    AnimatedSprite2D view;
    [Export]
    public EnumObjType objType = EnumObjType.Plans;
    public EnumObjType GetEnumObjType() => objType;
    [Export]
    public string objName = PlansConstants.XiHongShi;
    public string GetObjName() => objName;
    public bool Init(string name = null) => true;
    Area2D attackArea;
    Node2D fxNode;
    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        fxNode = GetNodeOrNull<Node2D>(NameConstants.Fx);
        attackArea = GetNodeOrNull<Area2D>(NameConstants.AttackArea);
        maxScale = Scale.X;
        minScale = ViewTool.GetYouMinScale(maxScale);
        damage = BulletConstants.GetPlansDamage(objName);
        
        CloseAttack();
    }
    bool canBeHurt = true;
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (!canBeHurt) return false;
        IHealth ih = view as IHealth;
        if (ih == null) return false;
        int yichu = ih.CostHealth(objType, damage, enumHurts);
        if (yichu > 0) __dieTime = InitAttackDelayTime;
        StartAttack(); 
        return true;
    }
    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts) => true;
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
            __t = 0.0001f;
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

    bool isAttacking = false;
    float __dieTime = 0f;
    float __attackTime = 0f;
    void StartAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        __attackTime = 0.0001f;
    }
    void Attack() {
        attackArea.Monitoring = true;
        attackArea.Monitorable = true;
        SoundFxController.Instance.PlayFx("Plans/" + objName, objName, 5);
        fxNode.Visible = true;
        AnimationTool.DoAniAttack(view);
        __dieTime = 0.0001f;
    }
    void CloseAttack()
    {
        attackArea.Monitoring = false;
        attackArea.Monitorable = false;
    }

    float BeHurtStartTime = 2.5f;
    float InitAttackDelayTime = 90f;
    float AttackAnimationTime = 0.3f;
    float __t = 0f;
    public override void _Process(double delta)
    {
        ViewTool.View3In1(this, minScale, maxScale);
        if (isWorking && __t > 0f)
        {
            __t += (float)delta;
            if (__t >= InitAttackDelayTime)
            {
                StartAttack();
                __t = 0f;
            }
        }

        if (__attackTime > 0f) {
            __attackTime += (float)delta;
            if (__attackTime >= BeHurtStartTime)
            {
                Attack();
                __attackTime = 0f;
            }
        }

        if (__dieTime > 0f)
        {
            __dieTime += (float)delta;
            if (__dieTime >= AttackAnimationTime)
            {
                fadeOutElapsed += (float)delta;
                float t = Mathf.Clamp(fadeOutElapsed / fadeOutDuration, 0f, 1f);
                // 线性插值透明度
                var mod = Modulate;
                mod.A = 1f - t;
                Modulate = mod;
                if (fadeOutElapsed >= fadeOutDuration)
                {
                    Die();
                    __dieTime = 0f;
                }
            }
        }
    }
    float fadeOutElapsed = 0f;
    float fadeOutDuration = 0.3f;

    public bool BeCure(EnumObjType objType, int cureAmount, EnumHurts enumHurts) => ObjTool.DoPlansCure(view as IHealth, objType, cureAmount);

}
