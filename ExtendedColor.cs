﻿using Microsoft.Xna.Framework;

namespace Blockaroz14Mod
{
    public struct ExtendedColor
    {
        public static Color ShadeColor = new Color(6, 7, 12);

        public static Color LightRed = Color.Lerp(Color.Red, Color.Coral, 0.36f);

        public static Color JellyOrange = Color.Lerp(Color.DarkRed, Color.DarkOrange, 0.5f);
    }
}