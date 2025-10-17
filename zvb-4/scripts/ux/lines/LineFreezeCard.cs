using Godot;
using System;
using System.Threading.Tasks;
using ZVB4.Conf;
using ZVB4.Interface;

public partial class LineFreezeCard : Node2D, IBeHurt
{
    [Export]
    public EnumRewords rewordType = EnumRewords.None;
    [Export]
    public int rewordAmount = 0;
    [Export]
    public int rewordViewScale = 1;
    [Export]
    public int InitHp = 20000;
    [Export]
    public string rewordPlanName = "";

    Sprite2D viewExtra;
	private Area2D _beHurtArea;
	private Label _hpLabel;
    private int _hp = 20000;

    Node2D sunCard;
    Node2D moneyCard;
    Node2D plansCard;

    public override void _Ready()
	{
		_beHurtArea = GetNodeOrNull<Area2D>(NameConstants.BeHurtArea);
		_hpLabel = GodotTool.FindCanvasItemByName(this, NameConstants.Label) as Label;
        UpdateHpLabel();
        viewExtra = GetNodeOrNull<Sprite2D>(NameConstants.View);
        _hp = InitHp;
        //
        sunCard = GetNodeOrNull<Node2D>("Sun");
        if (sunCard != null)
            sunCard.Visible = false;
        moneyCard = GetNodeOrNull<Node2D>("Money");
        if (moneyCard != null)
            moneyCard.Visible = false;
        plansCard = GetNodeOrNull<Node2D>("Plans");
        if (plansCard != null)
            plansCard.Visible = false;
        // 生成植物
        LoadRewordCard();
	}

    private void UpdateHpLabel()
    {
        if (_hpLabel != null)
            _hpLabel.Text = $"HP: {_hp}";
    }

    void GenerateNoPlans()
    {
        string rewordGroup = RewordConstants.GetRewordGroupScene("NoPlans");
        var scene = GD.Load<PackedScene>(rewordGroup);
        if (scene != null)
        {
            var instance = scene.Instantiate<Node2D>();
            if (instance != null)
            {
                RewordGroup rg = instance as RewordGroup;
                GD.Print("生成非植物奖励：" + rewordType.ToString() + " 数量：" + rewordAmount);
                if (rg != null) 
                {
                    rg.SpawnReword(rewordType.ToString(), rewordAmount, this.GlobalPosition, 1);
                }
                plansCard.AddChild(instance);
            }
        }
    }
    void GeneratePlans()
    {
        GD.Print("生成植物奖励：" + rewordPlanName);
        var py = PlayerController.Instance;
        if (py == null) return;
        if (PlansConstants.IsShooter(rewordPlanName))
        {
            py.LoadInitShooter(rewordPlanName);
        }
        else
        {
            var ms = RewordMiaoCenterSystem.Instance;
            if (ms != null)
            {
                ms.DumpPlansMiaoMust(GlobalPosition, rewordPlanName, true);
            }
        }
    }
    void GenerateRewordWhenDie() {
        if (rewordType == EnumRewords.None) return;
        if (rewordType == EnumRewords.Plans) {
            GeneratePlans();
            return;
        }
        GenerateNoPlans();
    }
    // 显示奖励卡
    void LoadRewordCard()
    {
        switch (rewordType)
        {
            case EnumRewords.Money:
                // 显示金钱奖励
                moneyCard.Visible = true;
                break;
            case EnumRewords.Sun:
                // 显示阳光奖励
                sunCard.Visible = true;
                break;
            case EnumRewords.Plans:
                // 显示植物卡片奖励
                plansCard.Visible = true;
                string scenePath = PlansConstants.GetPlanScene(rewordPlanName);
                // 生成植物
                var plantScene = GD.Load<PackedScene>(scenePath);
                if (plantScene != null)
                {
                    var plantInstance = plantScene.Instantiate<Node2D>();
                    if (plantInstance != null)
                    {
                        plansCard.AddChild(plantInstance);
                        plantInstance.Scale = new Vector2(rewordViewScale, rewordViewScale);
                        plantInstance.Position = new Vector2(0, 0);
                    }
                }
                break;
            default:
                break;
        }
    }
    
    public bool BeHurt(EnumObjType objType, int damage, EnumHurts enumHurts)
    {
        if (isDie) return true;
        _hp -= damage;
        // 受击打效果
        UpdateViewModulate();

        if (_hp < 0)
        {
            _hp = 0;
            isDie = true;
            Die(objType, damage, enumHurts);
        }
        UpdateHpLabel();
        return true;
    }

    public async Task<bool> Die(EnumObjType enumAttack, int damage, EnumHurts enumHurts)
    {
        _beHurtArea.Monitoring = false;
        GenerateRewordWhenDie();
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        QueueFree();
        return true;
    }
    private float _baseY = 0f;
    private bool _hasInitBaseY = false;
    bool isDie = false;
    void UpdateViewModulate()
    {
        if (viewExtra == null) return;
        float ratio = Mathf.Clamp((float)_hp / InitHp, 0f, 1f);
        // scale.y = ratio
        var scale = viewExtra.Scale;
        scale.Y = ratio;
        viewExtra.Scale = scale;
        // 保持底部对齐（锚点在底部中心）
        if (viewExtra.Texture != null)
        {
            float texHeight = viewExtra.Texture.GetHeight();
            // 以原始底部为基准，y = 原始y + texHeight * (1 - ratio) * 0.5f
            // 记录原始y
            if (!_hasInitBaseY)
            {
                _baseY = viewExtra.Position.Y;
                _hasInitBaseY = true;
            }
            float offset = texHeight * (1 - ratio) * 0.5f * viewExtra.Scale.X; // 乘以scale.x防止缩放异常
            viewExtra.Position = new Vector2(viewExtra.Position.X, _baseY + offset);
        }
    }
}
