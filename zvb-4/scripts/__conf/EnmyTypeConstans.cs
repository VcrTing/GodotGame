using System.Collections.Generic;
using Godot;

namespace ZVB4.Conf
{
    public static class EnmyTypeConstans
    {
        public const string ZombiM = "zombi_m"; // 1
        public const string ZombiS = "zombi_s"; // 2
        public const string ZombiJi = "zombi_ji"; // 0.5
        public const string ZombiGlq = "zombi_glq"; // 13
        public const string ZombiCgt = "zombi_cgt";
        public const string ZombiBaozhi = "zombi_baozhi"; // 5
        public const string ZombiMaozi = "zombi_maozi"; // 3
        public const string ZombiMuTong = "zombi_mu_tong"; // 4
        public const string ZombiTieTong = "zombi_tie_tong"; // 8
        public const string ZombiGangMen = "zombi_gang_men"; // 10
        public const string ZombiJuRen = "zombi_ju_ren"; // 30
        public const string ZombiMice = "zombi_mice"; // 0.5

        // 存储血量的字典
        public static readonly Dictionary<string, int> HpDict = new Dictionary<string, int>()
        {
            { ZombiM, (int)EnumHealth.One },
            { ZombiS, (int)EnumHealth.Two },
            { ZombiMaozi, (int)EnumHealth.Two },
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Two },
            { ZombiBaozhi, (int)EnumHealth.Three },
            { ZombiJi, (int)EnumHealth.Tiny },
            { ZombiGlq, (int)EnumHealth.Three },
            { ZombiGangMen, (int)EnumHealth.Two },
            { ZombiJuRen, (int)EnumHealth.JuRen },
            { ZombiMice, (int)EnumHealth.Tiny }
        };

        // 存储额外血量的字典
        public static readonly Dictionary<string, int> ExtraHpDict = new Dictionary<string, int>()
        {
            { ZombiMaozi, (int)EnumHealth.One },
            { ZombiMuTong, (int)EnumHealth.Two },
            { ZombiBaozhi, (int)EnumHealth.Two },
            { ZombiTieTong, (int)EnumHealth.Six },
            { ZombiGangMen, (int)EnumHealth.Eight },
            { ZombiGlq, (int)EnumHealth.Ten },
        };

        // 储存速度的字典
        public static readonly Dictionary<string, float> SpeedDict = new Dictionary<string, float>()
        {
            { ZombiCgt, 20f },
            { ZombiS, 20f },
            { ZombiMaozi, 30f },
            { ZombiMuTong, 15f },
            { ZombiTieTong, 10f },
            { ZombiBaozhi, 10f },
            { ZombiGangMen, 20f },

            { ZombiJi, 70f }, // run
            { ZombiM, 20f }, // run
            { ZombiGlq, 15f }, // run

            { ZombiJuRen, 7f },
            { ZombiMice, 50f}, // run
        };

        static int BaseDamage = (int)EnumHealth.One / 6;
        // 储存伤害的字典
        public static readonly Dictionary<string, int> DamageDict = new Dictionary<string, int>()
        {
            { ZombiJuRen, (int)EnumHealth.Ten },
            { ZombiMice, (int)EnumHealth.One }, 
        };
        public static float BaseBaitSpeed = 0.5f;
        public static float BaseBaitLazyStart = 0.5f;
        // 越小攻击越快
        public static readonly Dictionary<string, float> BaitSpeedDict = new Dictionary<string, float>()
        {
            { ZombiJuRen, BaseBaitSpeed * 3 },
            { ZombiM, BaseBaitSpeed / 2 },
            { ZombiJi, BaseBaitSpeed / 2 },
            { ZombiMice, BaseBaitSpeed / 2 },
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
            string n = "zombi_c_wrapper";
            if (enmyName.Contains("_mice")) { n = "zombi_t_wrapper"; }
            if (enmyName.Contains("_ju_ren")) { n = "zombi_x_wrapper"; }
            return FolderConstants.WaveEnemy + "wrapper/" + n + ".tscn";
        }
        public static bool GenerateZombiTexture(Node2D father, string key)
        {
            string folder = "texture/";
            if (key.Contains("_cgt") || key.Contains("_baozhi"))
            {
                folder = "texture_cc/";
            }
            if (key.Contains("_ju_ren") || key.Contains("_mice"))
            {
                folder = "texture_xt/";
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