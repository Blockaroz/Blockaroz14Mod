using Blockaroz14Mod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.NPCs
{
    public class FloatingDummy : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Dummy");
            NPCID.Sets.PositiveNPCTypesExcludedFromDeathTally[NPC.type] = true;
            NPCID.Sets.TeleportationImmune[NPC.type] = true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void SetDefaults()
        {
            Player player = Main.player[Main.myPlayer];
            NPC.width = 42;
            NPC.height = 38;
            NPC.aiStyle = 0;
            NPC.lifeMax = 100000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.spriteDirection = -player.direction;
            NPC.defense = 1;
            NPC.damage = 0;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.chaseable = true;
            NPC.lifeRegen = 90000;
            NPC.DeathSound = SoundID.NPCDeath22;
        }

        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];

            if (NPC.ai[0] < 60)
                NPC.ai[0]++;

            if (NPC.ai[0] == 60)
            {
                NPC.ai[0] = 0;
            }

            if (NPC.life < NPC.lifeMax)
                NPC.life += NPC.lifeMax;

            float length = Vector2.Distance(Main.MouseWorld, NPC.Center);

            if (length < 25 && player.altFunctionUse == 2)
                NPC.active = false;

            if (!NPC.active)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<BlackShadeDust>(), 0, 0, 0, Color.White, 0.66f)];
                    dust.noGravity = true;
                    Vector2 DustSpeed = new Vector2(i + 1f, 0f).RotatedBy(dust.position.AngleTo(NPC.Center));
                    dust.velocity = DustSpeed;
                    dust.position += NPC.velocity;
                }
            }

            Movement(player.MountedCenter, 0.5f);
        }

        public void Movement(Vector2 position, float speed = 1f)
        {
            if (Vector2.Distance(NPC.Center, position) > 550)
                NPC.velocity += NPC.DirectionTo(position) * speed;
            else
                SlowMovement();

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.9f;
        }

        public void SlowMovement()
        {
            NPC.velocity *= 0.95f;
            if (NPC.velocity.Length() < 0.005f)
                NPC.velocity = Vector2.Zero;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            float waveX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.09f;
            float waveY = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 10f) * 0.09f;
            Vector2 wobble = new Vector2(1 + waveX, 1 + waveY);
            Asset<Texture2D> BubbleTexture = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishBubble");

            for (int i = 0; i < 4; i++)
            {
                int DustType = ModContent.DustType<BlackShadeDust>();
                if (Main.rand.Next(2) == 1)
                    DustType = 264;
                Dust dust = Main.dust[Dust.NewDust(NPC.Center - BubbleTexture.Size() / 2, BubbleTexture.Width(), BubbleTexture.Height(), DustType, 0, 0, 0, Color.White, 0.66f)];
                dust.noGravity = true;
                Vector2 DustSpeed = new Vector2(i, 0f).RotatedBy(dust.position.AngleFrom(NPC.Center));
                dust.velocity = DustSpeed;
                dust.velocity *= 1.111f;
                dust.position += NPC.velocity;
            }

            Dust RainbowDust = Dust.NewDustDirect(NPC.position - new Vector2(7, 7), NPC.width + 14, NPC.height + 14, 267, 0, 0, 0, Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1f, 1f, 0.66f), 1f);
            RainbowDust.noGravity = true;
            Vector2 center = RainbowDust.position.DirectionTo(NPC.Center) * 2f;
            RainbowDust.velocity = center;
            RainbowDust.velocity *= 1.1f;
            RainbowDust.position += NPC.velocity / 2f;

            return true;
        }
    }
}