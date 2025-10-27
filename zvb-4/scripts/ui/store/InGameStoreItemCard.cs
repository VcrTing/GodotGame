using Godot;
using System;
using System.Diagnostics;
using ZVB4.Conf;

public partial class InGameStoreItemCard : Control
{
    [Export]
    public string ItemName = "Pea";
    [Export]
    public float ItemX = 155;
    [Export]
    public float ItemY = 75;
    [Export]
    public float ItemViewScale = 1f;

    string itemDescription = "豌豆射手";

    Node2D itemDisplayNode;

    int price = 100;

    Label DescLabel;
    Label PriceLabel;

    TextureButton buyButton;

    public override void _Ready()
    {
        itemDisplayNode = GodotTool.FindNode2DByName(this, NameConstants.View);
        DescLabel = GodotTool.FindLabelByName(this, "DescLabel");
        PriceLabel = GodotTool.FindLabelByName(this, "PriceLabel");
        buyButton = GodotTool.FindCanvasItemByName(this, "BuyButton") as TextureButton;
        if (buyButton != null)
        {
            buyButton.Pressed += OnBuyButtonPressed;
        }
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
        RewordMiaoCenterSystem.Instance.DumpPlansMiaoMust(new Vector2(0, 0), ItemName, false);
        SunCenterSystem.Instance.SubValue(price);
        StoreInGamePopup.Instance.HidePopup();
    }
    
    private void OnBuyButtonPressed()
    {
        if (__sure > 0f) return;
        __sure = 0.0001f;
        // 在这里实现购买逻辑
        int pp = SunCenterSystem.Instance.Value;
        if (pp >= price)
        {
            Buy();
        }
        else
        {
            FlashPriceLabelRed();
            SoundUiController.Instance.Error();
            return;
        }
    }

    public void Init(string name, int price, float x, float y, float viewscale)
    {
        ItemName = name;
        ItemX = x;
        ItemY = y;
        ItemViewScale = viewscale * 0.6f;
        this.price = price;
        // 根据ItemName加载和显示物品
        PlansConstants.GeneratePlans(itemDisplayNode, name);
        itemDisplayNode.Position = new Vector2(ItemX, ItemY);
        itemDisplayNode.Scale = new Vector2(ItemViewScale, ItemViewScale);
        itemDescription = I18nConstants.GetPlanChineseName(name);
        //
        DescLabel.Text = itemDescription;
        PriceLabel.Text = price.ToString();
    }

    public async void FlashPriceLabelRed()
    {
        if (PriceLabel == null) return;
        var oldModulate = PriceLabel.Modulate;
        PriceLabel.Modulate = new Color(1, 0, 0);
        await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
        PriceLabel.Modulate = oldModulate;
    }
}
