
using ZVB4.Conf;

namespace ZVB4.Entity
{

    [System.Serializable]
    public class EntityGameRunnerData
    {
        public float SoundLocation { get; set; } = 0f;

        public int CapterNumber { get; set; } = (int) EnumChapter.One1;
    }
}