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
    int itemId = 0;

    Label TitleLabel;
    Label DescLabel;
    Label PriceLabel;
    CanvasItem PriceIcon;

    TextureButton buyButton;

    public override void _Ready()
    {
        itemDisplayNode = GodotTool.FindNode2DByName(this, NameConstants.View);
        DescLabel = GodotTool.FindLabelByName(this, "DescLabel");
        TitleLabel = GodotTool.FindLabelByName(this, "TitleLabel");
        PriceLabel = GodotTool.FindLabelByName(this, "PriceLabel");
        PriceIcon = GodotTool.FindCanvasItemByName(this, "PriceIcon");
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
            issucc = StoreLoadCardSystem.Instance.SetItemBuyed(itemId, true);
            if (issucc)
            {
                __isBuyed = true;
                AsyncForBuyed();
            }
            return;
        }
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

    void AsyncForBuyed()
    {
        PriceLabel.Text = __isBuyed ? "已购买" : price.ToString();
        if (__isBuyed)
        {
            buyButton.Visible = false;
            PriceIcon.Visible = false;
        }
    }

    public void Init2(int id, int buyed, float x, float y, float viewscale)
    {
        if (id == 0) return;
        itemId = id;
        ItemX = x;
        ItemY = y;
        ItemViewScale = viewscale * 0.86f;
        itemDisplayNode.Position = new Vector2(ItemX, ItemY);
        itemDisplayNode.Scale = new Vector2(ItemViewScale, ItemViewScale);
        //
        __isBuyed = buyed == 1;
        AsyncForBuyed();
        init = true;
    }

    public void Init(string name, int price, string title, string desc)
    {
        ItemName = name;
        this.price = price;
        DescLabel.Text = desc;
        TitleLabel.Text = title;
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
}
