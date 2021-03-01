using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Projectiles.JellyfishProjs
{
    public class JellyfishBoltProj : ModProjectile
    {
        public override string Texture => "Blockaroz14Mod/Content/Projectiles/JellyfishProjs/JellyfishExplosionProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 720)
                SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);

            if (Projectile.timeLeft >= 360)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 15f;
            }

            if (Projectile.timeLeft <= 360)
            {
                Projectile.velocity *= 0.8f;
                Projectile.scale *= 0.97f;
            }

            if (Projectile.velocity.Length() < 0.01f)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, ExtendedColor.JellyOrange.ToVector3() * 0.3f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            


            return false;
        }
    }
}