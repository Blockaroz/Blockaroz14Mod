using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Blockaroz14Mod.Effects
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct NodachiDrawer
    {
        private static VertexStrip _vertexStrip = new VertexStrip();

        public void Draw(Projectile proj)
        {
            Player player = Main.player[proj.owner];
            MiscShaderData miscShaderData = GameShaders.Misc["Nodachi"];
            miscShaderData.UseShaderSpecificData(new Vector4(1f, 0f, 0f, 2f));
            miscShaderData.UseSaturation(3f);
            miscShaderData.UseOpacity(4f);
            miscShaderData.Apply();

            _vertexStrip.PrepareStrip(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, proj.oldPos.Length, includeBacksides: true);
            _vertexStrip.DrawTrail();

            if (proj.localAI[0] > 8 && proj.localAI[0] < 40)
            {
                Vector2 velocity = new Vector2(proj.direction == -1 ? -3 : 3, 0).RotatedBy(proj.rotation);

                Dust dust = Main.dust[Dust.NewDust(proj.Center - new Vector2(0f, 42f).RotatedBy(proj.rotation), 0, 0, DustID.FireworksRGB, velocity.X, velocity.Y, 0, ExtendedColor.LightRed, 0.5f)];
                dust.noGravity = true;
                dust.velocity = velocity;
                dust.position += dust.position.DirectionFrom(player.MountedCenter);
            }

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        private Color StripColors(float progressOnStrip)
        {
            float lerp = 1f - Terraria.Utils.GetLerpValue(0f, 0.98f, progressOnStrip);

            Color color1 = ExtendedColor.LightRed * lerp;
            color1.A /= 2;
            Color color2 = Color.Red * lerp;
            color2.A /= 3;
            Color result = Color.Lerp(color2, color1, 1f - Terraria.Utils.GetLerpValue(0.98f, 0f, lerp));
            return result;
        }

        private float StripWidth(float progressOnStrip)
        {
            float lerp = Terraria.Utils.GetLerpValue(0f, 1f, (float)Math.Sin(progressOnStrip), true);
            float width = MathHelper.Lerp(42f, 58f, lerp);
            return width;
        }
    }
}