using Blockaroz14Mod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Projectiles
{
    public class NodachiSlash : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.width = 10;
            Projectile.height = 90;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.manualDirectionChange = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 mountedCenter = player.MountedCenter;
            float lerpValue = Utils.GetLerpValue(900f, 0f, Projectile.velocity.Length() * 2f, clamped: true);
            float num = MathHelper.Lerp(0.7f, 2f, lerpValue);
            Projectile.localAI[0] += num;
            float TotalTime = 50f;
            if (Projectile.localAI[0] >= TotalTime * 2)
            {
                Projectile.Kill();
                return;
            }
            float CurrentTimeOverMax = Projectile.localAI[0] / TotalTime;
            float lerpValue2 = Utils.GetLerpValue(0f, 1f, CurrentTimeOverMax, clamped: true);
            float num3 = Projectile.ai[0];
            float num4 = Projectile.velocity.ToRotation();
            float PiFloat = (float)Math.PI;
            float dir = (Projectile.velocity.X > 0f) ? 1 : (-1);
            float num7 = PiFloat + dir * lerpValue2 * ((float)Math.PI * 2f);
            float num8 = Projectile.velocity.Length() + Utils.GetLerpValue(0.5f, 1f, lerpValue2, clamped: true) * 40f;
            if (num8 < TotalTime)
            {
                num8 = TotalTime;
            }
            Vector2 value = mountedCenter;
            Vector2 spinningpoint = new Vector2(1f, 0f).RotatedBy(num7) * new Vector2(num8, num3 * MathHelper.Lerp(2f, 1f, lerpValue));
            Vector2 value2 = value + spinningpoint.RotatedBy(num4);
            Vector2 value3 = (1f - Utils.GetLerpValue(0f, 0.5f, lerpValue2, clamped: true)) * new Vector2(((Projectile.velocity.X > 0f) ? 1 : (-1)) * (0f - num8) * 0.1f, (0f - Projectile.ai[0]) * 0.3f);
            float num10 = num7 + num4;
            Projectile.rotation = num10 + (float)Math.PI / 2f;
            Projectile.Center = value2 + value3;
            Projectile.spriteDirection = (Projectile.direction = ((Projectile.velocity.X > 0f) ? 1 : (-1)));
            if (num3 < 0f)
            {
                Projectile.rotation = PiFloat + dir * lerpValue2 * ((float)Math.PI * -2f) + num4;
                Projectile.rotation += (float)Math.PI / 2f;
                Projectile.spriteDirection = (Projectile.direction = ((!(Projectile.velocity.X > 0f)) ? 1 : (-1)));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle hitbox = new Rectangle(0, 0, 100, 100);
            float collisionPoint = 0f;
            float scaleFactor = 40f;
            for (int i = 14; i < Projectile.oldPos.Length; i += 15)
            {
                float num2 = Projectile.localAI[0] - i;
                if (!(num2 < 0f) && !(num2 > 60f))
                {
                    Vector2 value2 = Projectile.oldPos[i] + Projectile.Size / 2f;
                    Vector2 value3 = (Projectile.oldRot[i] + (float)Math.PI / 2f).ToRotationVector2();
                    hitbox.X = (int)value2.X - hitbox.Width / 2;
                    hitbox.Y = (int)value2.Y - hitbox.Height / 2;
                    if (hitbox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), value2 - value3 * scaleFactor, value2 + value3 * scaleFactor, 20f, ref collisionPoint))
                        return true;
                }
            }

            Vector2 value4 = (Projectile.rotation + (float)Math.PI / 2f).ToRotationVector2();
            hitbox.X = (int)Projectile.position.X - hitbox.Width / 2;
            hitbox.Y = (int)Projectile.position.Y - hitbox.Height / 2;
            if (hitbox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - value4 * scaleFactor, Projectile.Center + value4 * scaleFactor, 20f, ref collisionPoint))
                return true;

            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            default(NodachiDrawer).Draw(Projectile);

            for (int i = 0; i < Main.rand.Next(3, 6); i++)
            {
                float lerp = ExtendedUtils.GetSquareLerp(0f, 25f, 50f, Projectile.localAI[0] - i);
                Vector2 pos = Projectile.oldPos[i] + (Projectile.Size / 2) - new Vector2(0, 40 + i).RotatedBy(Projectile.oldRot[i]) - Main.screenPosition;

                ExtendedUtils.DrawSparkle(TextureAssets.Extra[98], SpriteEffects.None, pos, lerp * 0.7f, lerp * 0.6f, lerp * Main.rand.NextFloat(0.5f, 0.7f), lerp * Main.rand.NextFloat(1.5f, 2f), Projectile.oldRot[i] + MathHelper.PiOver2, ExtendedColor.LightRed, Color.White);
            }
            return false;
        }
    }
}