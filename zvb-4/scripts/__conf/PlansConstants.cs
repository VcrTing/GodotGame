
using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
using ZVB4.Interface;

namespace ZVB4.Conf
{
    public static class PlansConstants
    {
        public const string Miao = "Miao";
        // 植物名
        public const string Pea = "Pea";
        public const string PeaCold = "PeaCold";
        public const string PeaDouble = "PeaDouble";
        public const string YangTao = "YangTao";
        public const string LanMei = "LanMei";
        public const string ShiLiu = "ShiLiu";
        public const string Xigua = "Xigua";
        // 种植
        public const string SunFlower = "SunFlower";
        public const string JianGuo = "JianGuo";
        // 炸弹
        public const string IceFlower = "IceFlower";
        public const string Cherry = "Cherry";
        public const string LaJiao = "LaJiao";
        // 典藏
        public const string PeaGold = "PeaGold";
        public const string XiguaBing = "XiguaBing";
        // 植物名，卡片
        public static readonly Dictionary<string, string> PlanSceneDict = new Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlans + "plansshooter/pea.tscn" },
            { PeaCold, FolderConstants.WavePlans + "plansshooter/pea_cold.tscn" },
            { PeaDouble, FolderConstants.WavePlans + "plansshooter/pea_double.tscn" },

            { LanMei, FolderConstants.WavePlans + "plansshooter/lan_mei.tscn" },
            { YangTao, FolderConstants.WavePlans + "plansshooter/yang_tao.tscn" },
            { ShiLiu, FolderConstants.WavePlans + "plansshooter/shi_liu.tscn" },
            { Xigua, FolderConstants.WavePlans + "plansshooter/xigua.tscn" },

            { SunFlower, FolderConstants.WavePlans + "plansfuzhu/sun_flower.tscn" },
            { JianGuo,  FolderConstants.WavePlans + "planszhongzhi/jian_guo.tscn" },

            { LaJiao,  FolderConstants.WavePlans + "plansonce/la_jiao.tscn" },
            { Cherry,  FolderConstants.WavePlans + "plansonce/cherry.tscn" },
            { IceFlower,  FolderConstants.WavePlans + "plansonce/ice_flower.tscn" },

            { PeaGold, FolderConstants.WavePlans + "plansdiancang/pea_gold.tscn" },
            { XiguaBing, FolderConstants.WavePlans + "plansdiancang/xigua_bing.tscn" },
        };
        // 植物名，shooter
        public static readonly  Dictionary<string, string> ShooterDict = new  Dictionary<string, string>
        {
            { Pea, FolderConstants.WavePlayer + "shooter/shooter_pea.tscn" },
            { PeaCold, FolderConstants.WavePlayer + "shooter/shooter_pea_cold.tscn" },
            { PeaDouble, FolderConstants.WavePlayer + "shooter/shooter_pea_double.tscn" },

            { LanMei, FolderConstants.WavePlayer + "shooter/shooter_lan_mei.tscn" },
            { YangTao, FolderConstants.WavePlayer + "shooter/shooter_yang_tao.tscn" },
            { ShiLiu, FolderConstants.WavePlayer + "shooter/shooter_shi_liu.tscn" },
            { Xigua, FolderConstants.WavePlayer + "shooter/shooter_xigua.tscn" },
            //
            { PeaGold, FolderConstants.WavePlayer + "shooter_diancang/shooter_pea_gold.tscn" },
            { XiguaBing, FolderConstants.WavePlayer + "shooter_diancang/shooter_xigua_bing.tscn" },
        };
        // 植物生长时长
        public static readonly  Dictionary<string, float> PlanGrowTimeDict = new  Dictionary<string, float>
        {
            { Pea, 2f },
            { PeaCold, 2f },
            { PeaDouble, 4f },

            { LanMei, 3f },
            { YangTao, 5f },
            { ShiLiu, 1f },
            { Xigua, 4f },

            { SunFlower, 1f },
            { Cherry, 5f },
            { JianGuo, 2f },
            { IceFlower, 10f },
            { LaJiao, 3f },
            //
            { XiguaBing, 8f },
            { PeaGold, 8f },
        };
        // 植物生命值
        public static readonly  Dictionary<string, int> PlanHealthDict = new  Dictionary<string, int>
        {
            //  
            { SunFlower, (int)EnumHealth.Four },
            { JianGuo, (int)EnumHealth.JianGuo },
        };
        // 会占用格子的植物
        public static bool IsWillZhanYongGeZi(string planName)
        {
            if (planName == SunFlower || planName == JianGuo)
            {
                return true;
            }
            return false;
        }
        // 射手限制
        public static readonly  Dictionary<string, int> ShooterAttackLimitDict = new  Dictionary<string, int>
        {
            { Pea, 0 },
            { PeaCold, 24 },
            { PeaDouble, 20 },
            { Xigua, 12 },
            { LanMei, 14 },
            { YangTao, 14 },
            { ShiLiu, 16 },
            { XiguaBing, 8 },
            { PeaGold, 10 },
        };
        public static int GetShooterAttackLimit(string key) {
            if (ShooterAttackLimitDict.TryGetValue(key, out var value))
                return value;
            return 0;
        }
        // 获取植物攻击速度
        public static float GetPlansAttackSpeedStart(string key)
        {
            if (key == Pea) return 0.4f;
            else if (key == PeaCold) return 0.4f;
            else if (key == PeaDouble) return 0.5f;

            else if (key == LanMei) return 0.5f;
            else if (key == YangTao) return 0.36f;
            else if (key == ShiLiu) return 0.8f;
            else if (key == Xigua) return 1f;
            //
            if (key == PeaGold) return 2f;
            else if (key == XiguaBing) return 1f;
            return 0f;
        }
        public static float GetPlansAttackSpeedSnap(string key)
        {
            if (key == Pea) return 0.07f;
            else if (key == PeaCold) return 0.07f;
            else if (key == PeaDouble) return 0.07f;
            else if (key == LanMei) return 0.06f;
            else if (key == YangTao) return 0.05f;
            else if (key == ShiLiu) return 0.8f;
            else if (key == Xigua) return 0.8f;
            //
            if (key == PeaGold) return 0.08f;
            else if (key == XiguaBing) return 0.1f;
            return 0f;
        }
        public static float GetPlansAttackSpeedEnd(string key)
        {
            if (key == Pea) return 0.15f;
            else if (key == PeaCold) return 0.15f;
            else if (key == PeaDouble) return 0.18f;
            else if (key == LanMei) return 0.2f;
            else if (key == YangTao) return 0.2f;
            else if (key == ShiLiu) return 0.4f;
            else if (key == Xigua) return 0.5f;
            // 
            if (key == PeaGold) return 0.25f;
            else if (key == XiguaBing) return 0.2f;
            return 0f;
        }
        public static float GetPlansAttackSpeedSnapSnap(string key)
        {
            if (key == Pea) return 0.01f;
            else if (key == PeaCold) return 0.01f;
            else if (key == PeaDouble) return 0.01f;
            else if (key == LanMei) return 0.012f;
            else if (key == YangTao) return 0.012f;
            else if (key == ShiLiu) return 0.12f;
            else if (key == Xigua) return 0.01f;
            // 
            if (key == PeaGold) return 0.01f;
            if (key == XiguaBing) return 0.01f;
            return 0f;
        }

        //
        public static float GetRationSpeed(string key)
        {
            if (key == PeaGold) return 150f;
            if (key == ShiLiu) return 320f;
            if (key == Xigua) return 240f;
            return 0f;
        } 
        // 坚果倍数
        public static float BiggerRateMaxJianGuo = 1.5f;

        // 根据key获取植物生命值
        public static int GetPlansHealth(string key)
        {
            if (PlanHealthDict.TryGetValue(key, out var value))
                return value;
            return 0;
        }
        // 根据key获取场景路径
        public static string GetPlanScene(string key)
        {
            if (PlanSceneDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }
        // 根据key获取Shooter场景路径
        public static string GetShooterScene(string key)
        {
            if (ShooterDict.TryGetValue(key, out var value))
                return value;
            return string.Empty;
        }
        // 根据key获取生长时长
        public static float GetPlanGrowTime(string key)
        {
            if (PlanGrowTimeDict.TryGetValue(key, out var value))
                return value;
            return 0f;
        }
        public static bool IsShooter(string planName)
        {
            string v = GetShooterScene(planName);
            if (!string.IsNullOrEmpty(v))
                return true;
            return false;
        }
        public static Node2D GeneratePlans(Node2D father, string PlanName) {
            // 根据PlanName获取场景路径
            string scenePath = GetPlanScene(PlanName);
            if (!string.IsNullOrEmpty(scenePath))
            {
                var planScene = GD.Load<PackedScene>(scenePath);
                if (planScene != null)
                {
                    var planInstance = planScene.Instantiate();
                    IObj obj = planInstance as IObj;
                    if (obj != null)
                    {
                        obj.Init(PlanName);
                    }
                    father.AddChild(planInstance);
                    return planInstance as Node2D;
                }
            }
            return null;
        }
        // 
        public static Godot.Collections.Array GetAllPlanNamesArray()
        {
            var arr = new Godot.Collections.Array();
            foreach (var k in PlanSceneDict.Keys)
            {
                arr.Add(k);
            }
            return arr;
        }        
        public static Godot.Collections.Array GetSimplePlanNamesArray()
        {
            var arr = new Godot.Collections.Array();
            arr.Add(Pea);
            arr.Add(JianGuo);
            arr.Add(SunFlower);
            return arr;
        }
        //
        public static string GetRandomPlansName()
        {
            var keys = PlanSceneDict.Keys.ToList();
            var rand = new Random();
            string key = keys[rand.Next(keys.Count)];
            return key;
        }
        public static string GetRandomPlansName(Godot.Collections.Array allowList)
        {
            if (allowList == null || allowList.Count == 0)
            {
                return GetRandomPlansName();
            }
            var rand = new Random();
            string key = allowList[rand.Next(allowList.Count)].AsString();
            return key;
        }

        public static string GetShooterWrapperScenePath(string plansName)
        {
            if (plansName == PeaGold || plansName == ShiLiu || plansName == Xigua) {
                return FolderConstants.WavePlayer + "shooter_pao_wrapper.tscn";
            }
            return FolderConstants.WavePlayer + "shooter_gun_wrapper.tscn";
        }
    }

}