using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Dusts
{
    public class JellyfishBubbleDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            Color result = Color.White;
            result.A /= 2;
            return result;
        }
    }
}