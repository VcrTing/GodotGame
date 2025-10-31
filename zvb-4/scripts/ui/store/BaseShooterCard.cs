using Godot;
using System;
using ZVB4.Conf;

public partial class BaseShooterCard : Control
{
    
    [Export]
    public string ItemName = "Pea";
    [Export]
    public float ItemX = 100;
    [Export]
    public float ItemY = 100;
    [Export]
    public float ItemViewScale = 1f;

    string itemDescription = "豌豆射手";

    Node2D itemDisplayNode;

    int price = 100;
    int itemId = 0;

    Label PriceLabel;

    TextureButton buyButton;
    TextureButton touchButton;

    CanvasItem IsBuyedBar;
    CanvasItem NeedBuyBar;
    
    // TextureButton choiseButton;
    TextureButton choisedButton;

    public override void _Ready()
    {
        itemDisplayNode = GodotTool.FindNode2DByName(this, NameConstants.View);
        PriceLabel = GodotTool.FindLabelByName(this, "PriceLabel");
        //
        IsBuyedBar = GodotTool.FindCanvasItemByName(this, "IsBuyedBar");
        NeedBuyBar = GodotTool.FindCanvasItemByName(this, "NeedBuyBar");
        //
        touchButton = GodotTool.FindCanvasItemByName(this, "TouchButton") as TextureButton;
        // choiseButton = GodotTool.FindCanvasItemByName(IsBuyedBar, "ChoiseButton") as TextureButton;
        choisedButton = GodotTool.FindCanvasItemByName(IsBuyedBar, "ChoisedButton") as TextureButton;
        //
        buyButton = GodotTool.FindCanvasItemByName(this, "BuyButton") as TextureButton;
        if (buyButton != null) { buyButton.Pressed += OnBuyButtonPressed; }
        if (touchButton != null) { touchButton.Pressed += OnTouchButtonPressed; }
    }

    float __sure = 0f;
    public override void _Process(double delta)
    {

        if (__sure > 0f)
        {
            __sure -= (float)delta;
            if (__sure < 0.4f) __sure = 0f;
        }
    }

    void Buy()
    {
        if (MoneyCenterSystem.Instance == null) return;
        bool issucc = MoneyCenterSystem.Instance.CostForBuyed(price);
        if (!issucc)
        {
            FlashPriceLabelRed();
            SoundUiController.Instance.Error();
            return;
        }
        else
        {
            SoundUiController.Instance.Buyed();
            BaseShooterLoadSystem.Instance.BuyThisBaseShooter(ItemName);
            __isBuyed = true;
            AsyncForView();
            return;
        }
    }
    
    void OnTouchButtonPressed()
    {
        if (__sure > 0f) return;
        __sure = 0.0001f;
        if (!__isBuyed) {
            SoundUiController.Instance.Error();
            FlashPriceLabelRed();
            return;
        }
        if (!init) return;
        BaseShooterLoadSystem.Instance.SwitchBaseShooter(ItemName);
    }

    private void OnBuyButtonPressed()
    {
        if (__sure > 0f) return;
        __sure = 0.0001f;
        if (__isBuyed) return;
        if (!init) return;
        Buy();
    }

    bool init = false;

    void AsyncForView()
    {
        // 购买切换
        NeedBuyBar.Visible = !__isBuyed;
        IsBuyedBar.Visible = __isBuyed;
        // 选择切换
        string bs = BaseShooterLoadSystem.Instance.GetBaseShooter();
        choisedButton.Visible = (bs == ItemName);
        // choiseButton.Visible = !(bs == ItemName);
    }

    public void Init2(float x, float y, float viewscale)
    {
        ItemX = x;
        ItemY = y;
        ItemViewScale = viewscale * 1f;
        itemDisplayNode.Position = new Vector2(ItemX, ItemY);
        itemDisplayNode.Scale = new Vector2(ItemViewScale, ItemViewScale);
        __isBuyed = SaveDataManager.Instance.IsShooterUnLimit(ItemName);
        //GD.Print("是否购买" + __isBuyed + " " + ItemName);
        AsyncForView();
        init = true;
    }

    public void Init(string name, int price)
    {
        ItemName = name;
        this.price = price;
        PriceLabel.Text = price + "";
        // 根据ItemName加载和显示物品
        PlansConstants.GeneratePlans(itemDisplayNode, name);
    }

    bool __isBuyed = false;

    public async void FlashPriceLabelRed()
    {
        if (PriceLabel == null) return;
        var oldModulate = PriceLabel.Modulate;
        PriceLabel.Modulate = new Color(1, 0, 0);
        await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
        PriceLabel.Modulate = oldModulate;
    }

    public void ChangeChoise(string name) {
        // 购买切换
        NeedBuyBar.Visible = !__isBuyed;
        IsBuyedBar.Visible = __isBuyed;
        choisedButton.Visible = (name == ItemName);
        // choiseButton.Visible = !(name == ItemName);
    }
}
