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
		if (currentScene != null) currentScene.QueueFree();
        tree.ChangeSceneToFile(FolderConstants.Scenes + ChapterTool.MainMenuScene);
    }
    public static async void PrevScene(TextureButton node, string ScenePath)
    {
        try
        {
            SoundUiController.Instance.Back();
            // 0.2s后切换下一个场景
            await node.ToSignal(node.GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
            Node currentScene = node.GetTree().CurrentScene;
            node.GetTree().ChangeSceneToFile(ScenePath);
		    if (currentScene != null) currentScene.QueueFree();
        }
        catch (Exception ex)
        {
            GD.PrintErr("PrevScene error: " + ex.Message);
        }
    }
    public static async void NextScene(CanvasItem node, string ScenePath)
    {
        try
        {
            SoundUiController.Instance.Sure();
            // 0.2s后切换下一个场景
            await node.ToSignal(node.GetTree().CreateTimer(GameContants.UiLazyTouchTime), "timeout");
            Node currentScene = node.GetTree().CurrentScene;
            node.GetTree().ChangeSceneToFile(ScenePath);
            if (currentScene != null) currentScene.QueueFree();
        }
        catch (Exception ex)
        {
            GD.PrintErr("NextScene error: " + ex.Message);
        }
    }

    //
    public static void OpenBaseShooterOrStartGame(CanvasItem canvasItem, int capnum)
    {
        if (ChapterTool.NeedOpenBaseShooterPopup(capnum))
        {
            var bss = BaseShooterSwitcherPopup.Instance;
            if (bss != null && bss.IsAlive())
            {
                BaseShooterSwitcherPopup.Instance.ShowPopup();
                return;
            }
        }
        NextScene(canvasItem, FolderConstants.Scenes + ChapterTool.GetChapterSceneName(capnum));
    }
    
    public static async void Reload()
    {
        // Node currentScene = node.GetTree().CurrentScene;
    }
}
