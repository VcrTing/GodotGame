using Godot;
using System;
using ZVB4.Conf;
using ZVB4.Interface;
using ZVB4.Tool;

public partial class ThingsGuanZi : Node2D, IObj
{
    private Area2D _uxArea;
    private AnimatedSprite2D view;
    private AnimatedSprite2D view2;
    //
    public bool Die()
    {
        QueueFree();
        return true;
    }
    
    EnumObjType objType = EnumObjType.Things;
    public EnumObjType GetEnumObjType() => objType;

    string objName = "GuanZi";
    public string GetObjName() => objName;

    public bool Init(string name = null)
    {
        return true;
    }

    public override void _Ready()
    {
        view = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View);
        view2 = GetNodeOrNull<AnimatedSprite2D>(NameConstants.View + "2");
        _uxArea = GetNodeOrNull<Area2D>(NameConstants.UxArea);
        if (_uxArea != null)
        {
            _uxArea.Connect("input_event", new Callable(this, nameof(OnUxAreaInputEvent)));
        }
        //
        SwitchView();
    }

    void SwitchView() {
        if (view == null || view2 == null) return;
        if (viewcode == 1)
        {
            view.Visible = true;
            view2.Visible = false;
            AnimationTool.DoAniDefault(view);
        }
        else if (viewcode == 2)
        {
            view.Visible = false;
            view2.Visible = true;
            AnimationTool.DoAniDefault(view2);
        }
    }

    private void OnUxAreaInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        // 鼠标左键抬起
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (!mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            {
                OnUxAreaUp();
            }
        }
        // 触屏抬起
        else if (@event is InputEventScreenTouch touchEvent)
        {
            if (!touchEvent.Pressed)
            {
                OnUxAreaUp();
            }
        }
    }

    private async void OnUxAreaUp()
    {
        // TODO: 这里可以添加自定义逻辑
        AnimationTool.DoAniDie(view);
        AnimationTool.DoAniDie(view2);
        _uxArea.QueueFree();
        // 播放 
        SoundFxController.Instance?.PlayFx("Things/GuanZi", "GuanZi_die", 4);
        // 生成物品
        GenerateItem();
        //
        GameStatistic.Instance?.AddGuanziDie(1);
        //
        await ToSignal(GetTree().CreateTimer(0.618f), "timeout");
        // 销毁自己
        Die();
    }

    void GenerateItem()
    {
        // 产出一个植物
        if (plansName != "")
        {
            var ms = RewordMiaoCenterSystem.Instance;
            if (ms != null)
            {
                ms.DumpPlansMiao(Position, plansName, true);
            }
        }
        // 产出一个僵尸
        if (enmyName != "")
        {
            var eg = EnmyGenerator.Instance;
            if (eg != null)
            {
                eg.GenerateEnemyOfPos(Position, enmyName);
            }
        }
    }

    Godot.Collections.Array randomPlansList = new Godot.Collections.Array();
    Godot.Collections.Array randomEnemyList = new Godot.Collections.Array();
    float musenmyratio = 0f; // 僵尸概率
    string enmyName = "";
    string plansName = "";

    int viewcode = 1;

    public void Init(Vector2 pos, int viewCode, float musenmyratio, Godot.Collections.Array plansList, Godot.Collections.Array enemyList)
    {
        Position = pos;
        viewcode = viewCode;
        randomPlansList = plansList;
        randomEnemyList = enemyList;
        this.musenmyratio = musenmyratio;
        // 开始算物品
        float r = GD.RandRange(0, 100) / 100f;
        if (r < musenmyratio) {
            enmyName = GetEnmyName();
        } else {
            plansName = GetPlansName();
        }
        SwitchView();
    }
    string GetEnmyName()
    {
        if (randomEnemyList.Count == 0) return EnmyTypeConstans.ZombiS;
        string enemyName = (string)randomEnemyList[(int)GD.RandRange(0, randomEnemyList.Count - 1)];
        return enemyName;
    }
    string GetPlansName()
    {
        if (randomPlansList.Count == 0) return PlansConstants.SunFlower;
        string plansName = (string)randomPlansList[(int)GD.RandRange(0, randomPlansList.Count - 1)];
        return plansName;
    }
}
