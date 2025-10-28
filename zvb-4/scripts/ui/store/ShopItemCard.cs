using Godot;
using System;
using ZVB4.Conf;

public partial class ShopItemCard : Control
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

    Label TitleLabel;
    Label DescLabel;
    Label PriceLabel;

    TextureButton buyButton;

    public override void _Ready()
    {
        itemDisplayNode = GodotTool.FindNode2DByName(this, NameConstants.View);
        DescLabel = GodotTool.FindLabelByName(this, "DescLabel");
        TitleLabel = GodotTool.FindLabelByName(this, "TitleLabel");
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
        
    }

    private void OnBuyButtonPressed()
    {
        if (__sure > 0f) return;
        __sure = 0.0001f;
        if (__isBuyed) return;
        if (!init) return;
        // 在这里实现购买逻辑
        int pp = SunCenterSystem.Instance.Value;
        if (pp >= price)
        {
            //
        }
        else
        {
            FlashPriceLabelRed();
            SoundUiController.Instance.Error();
            return;
        }
    }

    bool init = false;

    public void Init(string name, int price, string title, string desc, float x, float y, float viewscale)
    {
        ItemName = name;
        ItemX = x;
        ItemY = y;
        ItemViewScale = viewscale * 0.86f;
        this.price = price;
        // 根据ItemName加载和显示物品
        PlansConstants.GeneratePlans(itemDisplayNode, name);
        itemDisplayNode.Position = new Vector2(ItemX, ItemY);
        itemDisplayNode.Scale = new Vector2(ItemViewScale, ItemViewScale);
        //
        PriceLabel.Text = __isBuyed ? "已购买" : price.ToString();
        TitleLabel.Text = title;
        DescLabel.Text = desc;
    }

    bool __isBuyed = false;

    public void SetIsBuyed(bool isBuyed)
    {
        __isBuyed = isBuyed;
        PriceLabel.Text = __isBuyed ? "已购买" : price.ToString();
        init = true;
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
