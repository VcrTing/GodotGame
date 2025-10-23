using System.Collections.Generic;
using Godot;

namespace ZVB4.Conf
{
    public static class EnmyTypeConstans
    {
        public const string ZombiM = "zombi_m";
        public const string ZombiS = "zombi_s";
        public const string ZombiJi = "zombi_ji";
        public const string ZombiGlq = "zombi_glq";
        public const string ZombiCgt = "zombi_cgt";
        public const string ZombiMuTong = "zombi_mu_tong";
        public const string ZombiTieTong = "zombi_tie_tong";

        // 存储血量的字典
        public static readonly Dictionary<string, int> HpDict = new Dictionary<string, int>()
        {
            { ZombiM, (int)EnumHealth.One },
            { ZombiS, (int)EnumHealth.Two },
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Two },

            { ZombiJi, (int)EnumHealth.Tiny },
            { ZombiGlq, (int)EnumHealth.Two },
        };

        // 存储额外血量的字典
        public static readonly Dictionary<string, int> ExtraHpDict = new Dictionary<string, int>()
        {
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Four },
            { ZombiGlq, (int)EnumHealth.Eight },
        };

        // 储存速度的字典
        public static readonly Dictionary<string, float> SpeedDict = new Dictionary<string, float>()
        {
            { ZombiJi, 70f },
            { ZombiM, 20f },
            { ZombiGlq, 20f },
            { ZombiCgt, 20f },
            { ZombiS, 20f },
            { ZombiMuTong, 15f },
            { ZombiTieTong, 10f }
        };

        static int BaseDamage = (int)EnumHealth.One / 6;
        // 储存伤害的字典
        public static readonly Dictionary<string, int> DamageDict = new Dictionary<string, int>()
        {
            
        };
        public static float BaseBaitSpeed = 0.5f;
        public static float BaseBaitLazyStart = 0.5f;
        public static readonly Dictionary<string, float> BaitSpeedDict = new Dictionary<string, float>()
        {
            { ZombiM, BaseBaitSpeed * 2 },
            { ZombiJi, BaseBaitSpeed * 2 },
        };

        public static float RedEyeScale = 3f;

        public static int GetZombieDamage(string key)
        {
            if (DamageDict.TryGetValue(key, out int value))
                return value;
            return BaseDamage;
        }
        public static float GetBaitSpeed(string key)
        {
            if (BaitSpeedDict.TryGetValue(key, out float value))
                return value;
            return BaseBaitSpeed;
        }
        public static float GetSpeed(string key)
        {
            if (SpeedDict.TryGetValue(key, out float value))
                return value;
            return 0f;
        }

        // 根据key获取血量
        public static int GetHp(string key)
        {
            if (HpDict.TryGetValue(key, out int value))
                return value;
            return 0;
        }

        // 根据key获取额外血量
        public static int GetExtraHp(string key)
        {
            if (ExtraHpDict.TryGetValue(key, out int value))
                return value;
            return 0;
        }

        // 
        public static string GetZombiWrapperScenePath(string enmyName)
        {
            return FolderConstants.WaveEnemy + "wrapper/zombi_c_wrapper.tscn";
        }
        public static bool GenerateZombiTexture(Node2D father, string key)
        {
            string folder = "texture/";
            if (key.Contains("_cgt")) {
                folder = "texture_cc/";
            }
            var textureScene = GD.Load<PackedScene>(FolderConstants.WaveEnemy + folder + key + "_texture.tscn");
            if (textureScene != null)
            {
                var textureInstance = textureScene.Instantiate<Node2D>();
                textureInstance.Name = NameConstants.Body;
                father.AddChild(textureInstance);
                return true;
            }
            return false;
        }
    }

}