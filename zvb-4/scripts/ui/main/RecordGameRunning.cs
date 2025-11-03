using Godot;
using System;

public partial class RecordGameRunning : Control
{   
    // 引用场景中的Label节点（需在编辑器中绑定或通过代码获取）
    private Label _fpsLabel;

    public override void _Ready()
    {
        // 获取Label节点（根据实际节点路径调整，这里假设Label与当前节点同层级）
        _fpsLabel = GodotTool.FindLabelByName(this, "LabelFrame");
    }

    public override void _Process(double delta)
    {
        // 获取当前帧率（整数，每秒更新一次）
        int currentFps = (int)Engine.GetFramesPerSecond();

        // 更新Label文本（格式化为"FPS: XXX"）
        _fpsLabel.Text = $"FPS: {currentFps}";
    }
}
