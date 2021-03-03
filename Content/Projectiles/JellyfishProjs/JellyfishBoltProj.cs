using Blockaroz14Mod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Projectiles.JellyfishProjs
{
    public class JellyfishBoltProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
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

            if (Projectile.timeLeft <= 360)
            {
                Projectile.velocity *= 0.88f;
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
            Vector2 origin = new Vector2(11, 11);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 30)
                Projectile.frameCounter = 0;

            float t = Utils.GetLerpValue(0, 30, Projectile.frameCounter) * MathHelper.TwoPi;

            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float rotation = t + i;
                float strength = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0, i, true);
                float rotOffset = MathHelper.ToRadians(Projectile.DirectionTo(Projectile.oldPos[i - 1]).ToRotation());
                Vector2 offset = new Vector2(0, 7 * strength).RotatedBy(rotation);
                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.oldPos[i] + (Projectile.Size / 2f) - offset - Main.screenPosition, origin, strength, 1f, 1f, Projectile.oldRot[i] + rotOffset, Color.DarkRed * 0.5f, ExtendedColor.JellyOrange);
            }

            for (int i = 0; i < 4; i++)
            {
                float rotation = t + i;
                Vector2 offset = new Vector2(0, 7).RotatedBy(rotation);
                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - offset - Main.screenPosition, origin, 1.5f, 1f, 1f, Projectile.rotation, Color.DarkRed * 0.5f, ExtendedColor.JellyOrange);
            }

            ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, origin, 1f, 1f, 1f, Projectile.rotation, ExtendedColor.JellyOrange, Color.LightGoldenrodYellow);

            Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<JellyExplosionDust>(), Projectile.velocity.X, Projectile.velocity.Y, 128, ExtendedColor.JellyOrange, 1.3f)];
            dust.noGravity = true;

            return false;
        }
    }
}