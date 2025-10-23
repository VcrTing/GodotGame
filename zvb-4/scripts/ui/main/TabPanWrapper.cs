using Godot;
using System;

public partial class TabPanWrapper : Control
{
	public static TabPanWrapper Instance { get; private set; }
	private Control[] _tabControls;

	public override void _Ready()
	{
		Instance = this;
		// 获取所有Control类型的直接子节点
		var children = GetChildren();
		var list = new System.Collections.Generic.List<Control>();
		foreach (var child in children)
		{
			if (child is Control ctrl)
				list.Add(ctrl);
		}
        _tabControls = list.ToArray();
		// 默认显示第一个Control
        ShowTabByIndex(1);
	}

	public void ShowTabByIndex(int idx)
    {
        if (idx < 1 || idx > _tabControls.Length)
            return;
		string targetName = $"Tab{idx}";
		GD.Print($"Switch tab: {targetName}");
		foreach (var ctrl in _tabControls)
		{
			if (ctrl.Name == targetName)
				ctrl.Visible = true;
			else
				ctrl.Visible = false;
		}
	}
}
