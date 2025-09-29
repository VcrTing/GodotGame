using System;
using System.Collections.Generic;
using Godot;
using ZVB4.Conf;

public static class UiTool {

    public static async void BackToMainScene(TextureButton node)
    {
        SoundUiController.Instance.Back();
        // 0.2s后切换下一个场景
        await node.ToSignal(node.GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
        var tree = node.GetTree();
		var currentScene = tree.CurrentScene;
		if (currentScene != null)
		{
			currentScene.QueueFree();
		}
        tree.ChangeSceneToFile(FolderConstants.Scenes + "main.tscn");
    }
    public static async void NextScene(TextureButton node, string ScenePath)
    {
        SoundUiController.Instance.Sure();
        // 0.2s后切换下一个场景
        await node.ToSignal(node.GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
        node.GetTree().ChangeSceneToFile(ScenePath);
    }
}
