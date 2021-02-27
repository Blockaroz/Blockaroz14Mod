using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Rarities
{
    public class LightRainbowRarity : ModRarity
    {
        private Color ColorMethod()
        {
            Color result = Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1f, 1f, 0.7f);
            result.A = 14;
            return result;
        }

        public override Color RarityColor => ColorMethod();
    }
}