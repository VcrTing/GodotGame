using Godot;
using System;
using ZVB4.Conf;

public partial class MenuOptionsButton : TextureButton
{
    [Export]
    public int Key = 1;
    [Export]
    public string LabelText = "Default Label";
    [Export]
    public bool IsActive = false;

    Label label;

	public override void _Ready()
	{
		this.Pressed += OnButtonPressed;
        ProcessMode = ProcessModeEnum.Always;
        this.MouseEntered += () => SoundUiController.Instance.Hover();
        // 获取Label子节点并设置文字
        label = GetNodeOrNull<Label>(NameConstants.Label);
        if (label != null)
        {
            label.Text = LabelText;
        }
	}

	private void OnButtonPressed()
	{
        GD.Print($"MenuOptionsButton clicked, Key: {Key}");
        SoundUiController.Instance.Sure();
        IsActive = true;
        if (IsActive)
        {
            // 常驻focus
            GrabFocus();
            //
            TabPanWrapper.Instance.ShowTabByIndex(Key);
            // Label 文字颜色为纯白
            // label?.AddThemeColorOverride("font_color", Colors.White);
        }
        else
        {
            // 可选：失去focus
            ReleaseFocus();
        }
	}
}
