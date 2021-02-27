using Terraria.Audio;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Sounds
{
    public class AestheticusNoise : ModSoundStyle
    {
        public AestheticusNoise(string modName, string soundPath, int variations = 0, SoundType type = SoundType.Sound, float volume = 1, float pitch = 0, float pitchVariance = 0) : base(modName, "Blockaroz14Mod/Sounds", variations, type, volume, pitch, pitchVariance)
        {
            //soundPath = "Blockaroz14Mod/Sounds/AestheticusNoise";
        }
    }
}