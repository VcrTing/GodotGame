
using System.Collections.Generic;
using Godot;

namespace ZVB4.Conf
{
    public static class I18nConstants
    {
        public static readonly Dictionary<string, string> PlanNameDict = new Dictionary<string, string>
        {
            { PlansConstants.Pea, "豌豆射手" },
            { PlansConstants.PeaCold, "寒冰豌豆" },
            { PlansConstants.PeaDouble, "双头豌豆" },

            { PlansConstants.LanMei, "蓝莓" },
            { PlansConstants.PaoGu, "紫泡泡菇" },
            { PlansConstants.YangTao, "杨桃" },
            { PlansConstants.ShiLiu, "石榴" },
            { PlansConstants.Xigua, "西瓜" },
            
            { PlansConstants.RewordFlower, "金钱花" },
            { PlansConstants.SunFlower, "阳光花" },
            { PlansConstants.SunGu, "阳光菇" },
            { PlansConstants.XianRenQiu, "仙人球" },
            { PlansConstants.JianGuo, "坚果墙" },

            { PlansConstants.Cherry, "樱桃丸子" },
            { PlansConstants.IceFlower, "冰冻花" },
            { PlansConstants.LaJiao, "十字辣椒" },
            { PlansConstants.XiHongShi, "西红柿" },

            { PlansConstants.Yezi, "椰子樽" },
            { PlansConstants.XiguaBing, "冰西瓜" },
            { PlansConstants.PeaGold, "海洋金枪" },
        };
        public static readonly Dictionary<string, string> TuJianJsonPaths = new Dictionary<string, string>
        {
            { PlansConstants.Pea, "tujian/plansshooter/Pea.json" },
            { PlansConstants.PeaCold, "tujian/plansshooter/PeaCold.json" },
            { PlansConstants.PeaDouble, "tujian/plansshooter/PeaDouble.json" },

            { PlansConstants.LanMei, "tujian/plansshooter/LanMei.json" },
            { PlansConstants.YangTao, "tujian/plansshooter/YangTao.json" },
            { PlansConstants.ShiLiu, "tujian/plansshooter/ShiLiu.json" },
            { PlansConstants.Xigua, "tujian/plansshooter/Xigua.json" },
            { PlansConstants.PaoGu, "tujian/plansshooter/PaoGu.json" },

            { PlansConstants.RewordFlower, "tujian/planszhongzhi/RewordFlower.json" },
            { PlansConstants.SunFlower, "tujian/planszhongzhi/SunFlower.json" },
            { PlansConstants.SunGu, "tujian/planszhongzhi/SunGu.json" },
            { PlansConstants.XianRenQiu, "tujian/planszhongzhi/XianRenQiu.json" },
            { PlansConstants.JianGuo, "tujian/planszhongzhi/JianGuo.json" },

            { PlansConstants.XiHongShi, "tujian/plansonce/XiHongShi.json" },
            { PlansConstants.Cherry, "tujian/plansonce/Cherry.json" },
            { PlansConstants.IceFlower, "tujian/plansonce/IceFlower.json" },
            { PlansConstants.LaJiao, "tujian/plansonce/LaJiao.json" },

            { PlansConstants.Yezi, "tujian/plansdiancang/Yezi.json" },
            { PlansConstants.XiguaBing, "tujian/plansdiancang/XiguaBing.json" },
            { PlansConstants.PeaGold, "tujian/plansdiancang/PeaGold.json" },
        };
        public static string GetPlanChineseName(string key)
        {
            if (PlanNameDict.TryGetValue(key, out var value))
                return value;
            return key;
        }
        public static string GetPlanTuJian(string key)
        {
            if (TuJianJsonPaths.TryGetValue(key, out var value))
                return value;
            return key;
        }

        public static void ShowTuJianPlansCard(CanvasItem father, string plansname, float viewLocX, float viewLocY, float scaleX)
        {
            string path = I18nConstants.GetPlanTuJian(plansname);
            if (path == null || path == "") return;
            path = FolderConstants.Designs + path;
            // 读取卡片数据（JSON -> Dictionary）
            Godot.Collections.Dictionary cardData = CommonTool.LoadJsonToDict(path);
            if (cardData != null)
            {
                var loader = GodotTool.FindCanvasItemByName(father, "TextureLoader");

                if (loader != null && cardData.ContainsKey("scenepath"))
                {
                    var scenePath = FolderConstants.WaveHouse + cardData["scenepath"].AsString();

                    if (!string.IsNullOrEmpty(scenePath))
                    {
                        var packedScene = GD.Load<PackedScene>(scenePath);
                        if (packedScene != null)
                        {
                            var instance = packedScene.Instantiate();
                            if (instance is Node2D node2D)
                            {
                                GodotTool.KillChildren(loader);
                                loader.AddChild(node2D);
                                node2D.Position = new Vector2(viewLocX, viewLocY);
                                node2D.Scale = new Vector2(node2D.Scale.X * scaleX, node2D.Scale.Y * scaleX);
                            }
                        }
                    }
                }
                var nameLabel = GodotTool.FindLabelByName(father, "LabelName");
                if (nameLabel != null && cardData.ContainsKey("nickname")) nameLabel.Text = cardData["nickname"].AsString();
                var infoLabel = GodotTool.FindLabelByName(father, "LabelInfo");
                if (infoLabel != null && cardData.ContainsKey("info")) infoLabel.Text = cardData["info"].AsString();
                var descLabel = GodotTool.FindLabelByName(father, "LabelDesc");
                if (descLabel != null && cardData.ContainsKey("description")) descLabel.Text = cardData["description"].AsString();
            }
        }
    }
}