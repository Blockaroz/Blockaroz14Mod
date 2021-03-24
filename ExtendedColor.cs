using Microsoft.Xna.Framework;

namespace Blockaroz14Mod
{
    public struct ExtendedColor
    {
        public static readonly Color ShadeColor = new Color(6, 7, 12);

        public static readonly Color LightRed = Color.Lerp(Color.Red, Color.Coral, 0.36f);

        public static readonly Color JellyOrange = new Color(255, 80, 0);
    }
}