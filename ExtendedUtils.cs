using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace Blockaroz14Mod
{
    public class ExtendedUtils
    {
        /// <summary>
        /// Modular sparkle drawer. Works best with symmetrical textures.
        /// </summary>
        public static void DrawSparkle(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float thickness, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte colorAlpha = 0)
        {
            Color color1 = drawColor * opacity;
            color1.A = colorAlpha;
            Color color2 = shineColor * opacity;
            color2.A = colorAlpha;

            Vector2 vector1 = new Vector2(thickness, width) * scale;
            Vector2 vector2 = new Vector2(thickness, height) * scale;

            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation, origin, vector1, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation + MathHelper.PiOver2, origin, vector2, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation, origin, vector1 * 0.56f, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation + MathHelper.PiOver2, origin, vector2 * 0.56f, dir, 0);
        }

        /// <summary>
        /// Useful for sparkles that don't use symmetrical textures.
        /// </summary>
        public static void DrawStreak(Asset<Texture2D> texture, SpriteEffects dir, Vector2 drawCenter, Vector2 origin, float scale, float width, float height, float rotation, Color drawColor, Color shineColor, float opacity = 1f, byte colorAlpha = 0)
        {
            Color color1 = drawColor * opacity;
            color1.A = colorAlpha;
            Color color2 = shineColor * opacity;
            color2.A = colorAlpha;

            Vector2 vector = new Vector2(width, height) * scale;

            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color1, rotation + MathHelper.PiOver2, origin, vector, dir, 0);
            Main.EntitySpriteDraw(texture.Value, drawCenter, null, color2, rotation + MathHelper.PiOver2, origin, vector * 0.56f, dir, 0);
        }

        public static float GetSquareLerp(float start, float middle, float end, float value)
        {
            return Utils.GetLerpValue(start, middle, value, true) * Utils.GetLerpValue(end, middle, value, true);
        }

        public static float GetSquareLerp(float start, float middleOne, float middleTwo, float end, float value)
        {
            return Utils.GetLerpValue(start, middleOne, value, true) * Utils.GetLerpValue(end, middleTwo, value, true);
        }

        public static float GetCircle(float counter, float total, float piMultiplier = 1f)
        {
            return ((MathHelper.TwoPi * piMultiplier) / (total)) * counter;
        }

        public static Vector2 GetPositionAroundTarget(Vector2 center, float radius, bool careAboutTiles)
        {
            Vector2 rotation = -(Vector2.UnitY * radius).RotatedByRandom(MathHelper.Pi);

            while (careAboutTiles == true)
            {
                Vector2 position = center + rotation;
                if (!Collision.SolidCollision(position, 1, 1))
                {
                    return position;
                }
                else
                {
                    rotation = rotation.RotatedByRandom(MathHelper.Pi);
                }
            }
            return center + rotation;
        }
    }
}