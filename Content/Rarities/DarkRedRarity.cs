﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Rarities
{
    public class DarkRedRarity : ModRarity
    {
        private Color ColorMethod()
        {
            float t = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.21f));
            float lerp = ExtendedUtils.GetSquareLerp(0.2f, 0.35f, 0.5f, t);
            Color result = Color.Lerp(ExtendedColor.LightRed, ExtendedColor.ShadeColor, lerp);
            return result;
        }

        public override Color RarityColor => ColorMethod();
    }
}