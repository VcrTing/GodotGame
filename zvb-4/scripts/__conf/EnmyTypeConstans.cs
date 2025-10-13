using System.Collections.Generic;
using Godot;

namespace ZVB4.Conf
{
    public static class EnmyTypeConstans
    {
        public const string ZombiM = "zombi_m";
        public const string ZombiS = "zombi_s";
        public const string ZombiMuTong = "zombi_mu_tong";
        public const string ZombiTieTong = "zombi_tie_tong";

        // 存储血量的字典
        public static readonly Dictionary<string, int> HpDict = new Dictionary<string, int>()
        {
            { ZombiM, (int)EnumHealth.One },
            { ZombiS, (int)EnumHealth.Two },
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Two }
        };

        // 存储额外血量的字典
        public static readonly Dictionary<string, int> ExtraHpDict = new Dictionary<string, int>()
        {
            { ZombiM, (int)EnumHealth.Zero },
            { ZombiS, (int)EnumHealth.Zero },
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Four }
        };

        // 储存速度的字典
        public static readonly Dictionary<string, float> SpeedDict = new Dictionary<string, float>()
        {
            { ZombiM, 40f },
            { ZombiS, 20f },
            { ZombiMuTong, 15f },
            { ZombiTieTong, 10f }
        };

        static int BaseDamage = (int)EnumHealth.One / 6;
        // 储存伤害的字典
        public static readonly Dictionary<string, int> DamageDict = new Dictionary<string, int>()
        {
            { ZombiM, BaseDamage },
            { ZombiS, BaseDamage },
            { ZombiMuTong, BaseDamage  },
            { ZombiTieTong, BaseDamage  }
        };
        public static float BaseBaitSpeed = 0.5f;
        public static float BaseBaitLazyStart = 0.5f;
        public static readonly Dictionary<string, float> BaitSpeedDict = new Dictionary<string, float>()
        {
            { ZombiM, BaseBaitSpeed * 2 },
            { ZombiS, BaseBaitSpeed },
            { ZombiMuTong, BaseBaitSpeed  },
            { ZombiTieTong, BaseBaitSpeed  }
        };
        public static int GetZombieDamage(string key)
        {
            if (DamageDict.TryGetValue(key, out int value))
                return value;
            return 0;
        }
        public static float GetBaitSpeed(string key)
        {
            if (BaitSpeedDict.TryGetValue(key, out float value))
                return value;
            return 0f;
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
        public static bool GenerateZombiTexture(Node2D father, string key)
        {
            var textureScene = GD.Load<PackedScene>(FolderConstants.WaveEnemy + "texture/" + key + "_texture.tscn");
            if (textureScene != null)
            {
                var textureInstance = textureScene.Instantiate<Node2D>();
                father.AddChild(textureInstance);
                return true;
            }
            return false;
        }
    }

}