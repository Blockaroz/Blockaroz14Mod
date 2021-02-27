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
using Terraria.Utilities;

namespace Blockaroz14Mod.Content.Projectiles.JellyfishProjs
{
    public class JellyfishLightningProj : ModProjectile
    {
        public override string Texture => "Blockaroz14Mod/Content/Projectiles/JellyfishProjs/JellyfishExplosionProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 720;
        }

        private float random = 1f;

        public override void AI()
        {
            /*if (Projectile.timeLeft >= 360)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 10f;
            }*/

            if (Projectile.timeLeft <= 360)
            {
                Projectile.velocity *= 0.88f;
                Projectile.scale *= 0.96f;
            }

            if (Projectile.velocity.Length() < 0.01f)
                Projectile.Kill();

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= Main.rand.Next(15, 30))
            {
                if (random != 0)
                    random *= -1f;

                Projectile.velocity = Vector2.Zero;
                Projectile.velocity = Projectile.oldVelocity.RotatedBy(random);
                Projectile.localAI[0] = 0;
                return;
            }
            if (Projectile.timeLeft >= 360)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= 8;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Collision.SolidCollision(Projectile.Center, Projectile.width, Projectile.height))
                Projectile.velocity = Vector2.Zero;

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < 25; i++)
            {
                if (targetHitbox.Contains(Projectile.oldPos[i].ToPoint()))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Asset<Texture2D> texture = ModContent.GetTexture("Blockaroz14Mod/Assets/Streak_" + (short)1);

            ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.Center - Main.screenPosition, Projectile.scale * 0.9f, 1f, 2f, Projectile.rotation, ExtendedColor.JellyOrange, Color.LightGoldenrodYellow);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                ExtendedUtils.DrawStreak(texture, SpriteEffects.None, Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition, Projectile.scale * 0.5f, 1f, 2f, Projectile.oldRot[i], ExtendedColor.JellyOrange, Color.DarkGoldenrod);

                float lightStrength = Utils.GetLerpValue(ProjectileID.Sets.TrailCacheLength[Type], 0.1f, i, true);
                Lighting.AddLight(Projectile.Center, ExtendedColor.JellyOrange.ToVector3() * 0.3f * lightStrength);
            }

            if (Projectile.timeLeft >= 705)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<JellyExplosionDust>(), Projectile.velocity.X, Projectile.velocity.Y, 128, ExtendedColor.JellyOrange, 1.3f)];
                dust.noGravity = true;
                dust.velocity *= 1.4f;
            }

            return false;
        }
    }
}