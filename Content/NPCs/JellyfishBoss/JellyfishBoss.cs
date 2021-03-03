using Blockaroz14Mod.Content.Dusts;
using Blockaroz14Mod.Content.Projectiles.JellyfishProjs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.NPCs.JellyfishBoss
{
    [AutoloadBossHead]
    public class JellyfishBoss : ModNPC
    {
        public override string Texture => "Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishBubble";

        public override string BossHeadTexture => "Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishBoss_Head";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jellyfish");
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.Frostburn,
                    BuffID.Bleeding
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.width = 158;
            NPC.height = 150;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath22;

            Music = MusicID.OtherworldlyBoss1;
            NPC.lifeMax = 7000;
            NPC.lifeRegen = 1;
            NPC.defense = 30;
            NPC.value = Item.buyPrice(gold: 4);
            NPC.SpawnWithHigherTime(30);
            NPC.knockBackResist = 0.2f;
            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(15f);
        }

        public ref float Phase => ref NPC.ai[0];
        public ref float PhaseTimer => ref NPC.ai[1];
        public ref float OverallPhase => ref NPC.ai[2];
        public ref float DrawPhase => ref NPC.ai[3];
        public bool TeleportNext = false;
        public ref float LocalCounter => ref NPC.localAI[0];
        public ref float ExtraCounter => ref NPC.localAI[1];

        public static float PhaseTimerMax = 60;
        public static float TeleportTime = PhaseTimerMax;
        public static float BaseAttackTime = 100;
        public static float TrailAttackTime = 20;
        public static float TrailAttackTimeMax = 140;

        public static int BubbleDust = ModContent.DustType<JellyfishBubbleDust>();
        public static int JellyExplodeDust = ModContent.DustType<JellyExplosionDust>();

        public static int CoreOffset = 60;

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                NPC.velocity.Y += 0.04f;

                NPC.EncourageDespawn(12);
                return;
            }

            PhaseCheck();

            if (OverallPhase <= 2)
            {
                PhaseTimer++;
                if (PhaseTimer > PhaseTimerMax)
                    PhaseTimer = PhaseTimerMax;

                if (Phase <= 0)
                    Phase++;

                if (Phase == 1 && PhaseTimer == PhaseTimerMax && TeleportNext == false)
                    BaseAttack(player);

                else if (Phase == 1 && PhaseTimer == PhaseTimerMax && TeleportNext == true)
                    SpinBaseAttack(player);

                else if (Phase == 2 && PhaseTimer == PhaseTimerMax)
                    ExplosionAttack(player, true, OverallPhase == 2);

                else if (Phase == 3 && PhaseTimer == PhaseTimerMax && OverallPhase == 2)
                    LightningCircleAttack(player);

                else if (TeleportNext == true)
                    Teleport(player);

                else
                {
                    DrawPhase = 0;
                    Movement(player.MountedCenter, 0.55f, 10);
                }
            }
            else if (OverallPhase == 3)
            {
                NPC.immortal = true;
                Slow(0.8f);
                DrawPhase = -3;

                DustHelper(true, 1f, BubbleDust);
                DustHelper(false, 1.2f, JellyExplodeDust);

                PhaseTimer++;
                for (int i = 0; i < 180; i++)
                {
                    if (PhaseTimer == i * 10)
                    {
                        ExplosionAttack(player, false, true);
                    }
                }
                if (PhaseTimer >= 180)
                {
                    DrawPhase = 0;
                    NPC.immortal = false;
                    PhaseTimer = 0;
                    OverallPhase = 4;
                    ResetAI(true);

                    DustHelper(true, 1f, JellyExplodeDust);
                    for (int i = 0; i < 3; i++)
                        DustHelper(false, 1f, JellyExplodeDust);
                }
            }
            else if (OverallPhase == 4)
            {
                NPC.defense = 50;
                PhaseTimer++;
                if (PhaseTimer > PhaseTimerMax)
                    PhaseTimer = PhaseTimerMax;

                if (Phase <= 0)
                    Phase++;

                if (Phase == 1 && PhaseTimer == PhaseTimerMax && TeleportNext == false)
                    BaseAttackTwo(player);

                else if (Phase == 2 && PhaseTimer == PhaseTimerMax)
                    SpinExplosionAttack(player);

                else if (Phase == 3 && PhaseTimer == PhaseTimerMax)
                    ExplosionAttack(player, true, false);

                else if (Phase == 4 && PhaseTimer == PhaseTimerMax)
                    ResetAI(true);

                else if (TeleportNext == true)
                    Teleport(player);

                else if (TeleportNext == false)
                    Movement(player.MountedCenter, 0.66f, 20);
            }
        }

        public static int GetScaledNumbers(int count)
        {
            if (Main.expertMode)
                count += 15;

            if (Main.getGoodWorld)
                count += 15;

            return count;
        }

        public static Vector2 GetPredictedPosition(Player player, float intensity) => player.Center + (player.velocity * 2f * intensity);

        public void PhaseCheck()
        {
            if (OverallPhase == 4)
            {
                return;
            }
            else if (NPC.GetLifePercent() < 0.3f)
            {
                OverallPhase = 3;
                return;
            }
            else if (NPC.GetLifePercent() < 0.6f)
            {
                OverallPhase = 2;
                return;
            }
            else
            {
                OverallPhase = 1;
                return;
            }
        }

        public void ResetAI(bool resetPhase)
        {
            if (resetPhase)
                Phase = -1;
            else
                Phase++;
            PhaseTimer = 0;
            LocalCounter = 0;
        }

        public void Movement(Vector2 position, float speed, float minDistance)
        {
            if (Vector2.Distance(NPC.Center, position) >= minDistance)
                NPC.velocity += NPC.DirectionTo(position) * speed;
            else
                Slow(0.97f);

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.93f;
        }

        public void Slow(float harshness = 0.933f)
        {
            NPC.velocity *= harshness;
            if (NPC.velocity.Length() < 0.0044f)
                NPC.velocity = Vector2.Zero;
        }
 
        public void Teleport(Player player)
        {
            DrawPhase = -1;

            Slow();

            Vector2 pos = ExtendedUtils.GetPositionAroundTarget(player.MountedCenter, Main.rand.Next(370, 460), true);

            ExtraCounter++;

            NPC.position += player.velocity / 10f;

            if (ExtraCounter == (TeleportTime / 2))
            {
                NPC.position = pos;
            }
            if (ExtraCounter >= TeleportTime)
            {
                ExtraCounter = 0;
                DrawPhase = 0;
                TeleportNext = false;
                return;
            }
        }

        public void StraightDash(Vector2 vector, float speed, bool instant = false)
        {
            DrawPhase = -2;

            NPC.knockBackResist = 0f;

            if (instant == false)
            {
                ExtraCounter++;
                if (ExtraCounter < 15)
                {
                    Slow(0.75f);
                    NPC.velocity -= NPC.DirectionTo(vector) * (speed / 5);
                    DustHelper(true, 1f, BubbleDust);
                }
                if (ExtraCounter == 25)
                {
                    NPC.velocity += NPC.DirectionTo(vector) * speed;
                    NPC.velocity.Normalize();
                    NPC.velocity *= speed;
                }
            }
            else
            {
                NPC.velocity += NPC.DirectionTo(vector) * speed;
                NPC.velocity.Normalize();
                NPC.velocity *= speed;
                return;
            }

            if (ExtraCounter >= 27)
            {
                ExtraCounter = 0;
                DrawPhase = 0;
                NPC.knockBackResist = 0.7f;
                Slow(0.84f);
                return;
            }
        }

        public void BaseAttack(Player player)
        {
            Movement(player.MountedCenter, 0.33f, 30);

            LocalCounter++;

            for (int i = 0; i < (BaseAttackTime / 10); i++)
            {
                if (LocalCounter == 10 * i && LocalCounter > 20)
                {
                    float rotation = NPC.Center.DirectionTo(player.MountedCenter).ToRotation();
                    Vector2 offset = new Vector2(CoreOffset, 0).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, NPC.Center.DirectionTo(player.MountedCenter).RotatedByRandom(0.1f) * (10f + i), ModContent.ProjectileType<JellyfishBoltProj>(), NPC.damage, 0);
                }
            }
            if (LocalCounter >= BaseAttackTime)
            {
                ResetAI(false);
                return;
            }
        }

        public void BaseAttackTwo(Player player)
        {
            Movement(player.MountedCenter, 0.13f, 30);

            LocalCounter++;

            for (int i = 0; i < (84 / 7); i++)
            {
                if (LocalCounter == 7 * i && LocalCounter > 5)
                {
                    SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                    float rotation = NPC.Center.DirectionTo(player.MountedCenter).ToRotation();
                    Vector2 offset = new Vector2(CoreOffset, 0).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, NPC.Center.DirectionTo(player.MountedCenter).RotatedBy(1) * 5f, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.damage, 0);
                }
            }
            if (LocalCounter >= 84)
            {
                ResetAI(false);
                return;
            }
        }

        public void SpinBaseAttack(Player player)
        {
            LocalCounter++;
            int TotalProjectiles = GetScaledNumbers(30);

            for (int i = 0; i < TotalProjectiles; i++)
            {
                if (LocalCounter == (i * 3))
                {
                    float rotation = ExtendedUtils.GetCircle(i * 3, TotalProjectiles) + (Main.rand.NextFloat(-3, 3) / 30);
                    Vector2 offset = new Vector2(0, CoreOffset).RotatedBy(rotation);
                    Vector2 speed = new Vector2(0, Main.rand.Next(8, 12)).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, speed, ModContent.ProjectileType<JellyfishBoltProj>(), NPC.damage, 0);
                }
            }

            if (LocalCounter > 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 1f, 30);
            }
            else
                DrawPhase = -3;

            if (LocalCounter >= 60)
            {
                ResetAI(false);
                return;
            }
        }

        public void ExplosionAttack(Player player, bool aroundPlayer, bool onPlayer)
        {
            LocalCounter++;
            if (LocalCounter > 15)
                Movement(player.MountedCenter, 0.15f, 70);
            else
                Slow();

            int playerCount = Main.player.Count(x => x.active);
            for (int i = 0; i < playerCount; i++)
            {
                Player target = Main.player[i];
                for (int j = 0; j < TrailAttackTimeMax; j++)
                {
                    float multiplier;
                    if (Main.expertMode)
                        multiplier = j * 0.5f;
                    else if (Main.getGoodWorld)
                        multiplier = 1f;
                    else
                        multiplier = j;

                    if (LocalCounter == TrailAttackTime * multiplier && aroundPlayer)
                    {
                        Vector2 pos1 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(60, 720), true);
                        Projectile.NewProjectile(pos1, pos1.DirectionTo(target.MountedCenter), ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.damage, 0);
                    }

                    if (LocalCounter == (TrailAttackTime * multiplier * 2f) + (TrailAttackTime * 0.5f) && onPlayer)
                    {
                        Vector2 pos2 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(5, 60), false);
                        Projectile.NewProjectile(pos2, pos2.DirectionTo(target.MountedCenter), ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.damage, 0);
                    }
                }
            }

            if (LocalCounter >= TrailAttackTimeMax)
            {
                if (Main.rand.Next(2) == 0 && OverallPhase == 1)
                    TeleportNext = true;

                if (OverallPhase > 1)
                    ResetAI(false);
                else
                    ResetAI(true);

                return;
            }
        }

        public void SpinExplosionAttack(Player player)
        {
            LocalCounter++;
            int TotalProjectiles = GetScaledNumbers(10);

            for (int i = 0; i < TotalProjectiles; i++)
            {
                if (LocalCounter == (i * 3) && LocalCounter >= 30)
                {
                    float rotation = ExtendedUtils.GetCircle(i * 3, TotalProjectiles) + (Main.rand.NextFloat(-3, 3) / 30);
                    Vector2 offset = new Vector2(0, CoreOffset).RotatedBy(rotation);
                    Vector2 speed = new Vector2(0, Main.rand.Next(11, 15)).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, speed, ModContent.ProjectileType<JellyfishMovingExplosionProj>(), NPC.damage, 0);

                    Projectile.NewProjectile(ExtendedUtils.GetPositionAroundTarget(NPC.Center, Main.rand.Next(60, 720), true), Vector2.Zero, ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.damage, 0);
                }
            }

            if (LocalCounter > 70)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 0.89f, 30);
            }
            else
                DrawPhase = -3;

            if (LocalCounter <= 30)
                Movement(player.MountedCenter, 2f, 400);
            else if (LocalCounter <= 70)
                Slow(0.85f);

            if (LocalCounter >= 90)
            {
                ResetAI(false);
                return;
            }
        }

        public void LightningCircleAttack(Player player)
        {
            int TotalExplosionProjectiles = GetScaledNumbers(10);

            DrawPhase = -3;

            Slow(0.8f);

            LocalCounter++;

            if (LocalCounter < 25)
            {
                DustHelper(true, 1f, BubbleDust);
                DustHelper(true, 1.2f, JellyExplodeDust);
            }
            if (LocalCounter == 30)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                for (int i = 0; i < TotalExplosionProjectiles; i++)
                {
                    DustHelper(false, 1f, BubbleDust);
                    Vector2 speed = new Vector2(0, Main.rand.Next(5, 6)).RotatedBy(ExtendedUtils.GetCircle(i, TotalExplosionProjectiles)).RotatedByRandom(0.3f);
                    Projectile.NewProjectile(NPC.Center, speed, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.damage, 0);
                }
            }

            if (LocalCounter >= 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 0.7f, 30);
            }
            if (LocalCounter >= 70)
            {
                if (Main.rand.Next(2) == 0 && OverallPhase == 2)
                    TeleportNext = true;

                ResetAI(true);
                return;
            }
        }

        public void DustHelper(bool inward, float scale, int type)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 DustSpeed;
                Vector2 DustPosition = NPC.position - new Vector2(10, 10);
                Dust dust = Main.dust[Dust.NewDust(DustPosition, NPC.width + 20, NPC.height + 20, type, 0, 0, 100, Color.White, scale)];
                if (inward)
                {
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleTo(NPC.Center + new Vector2(0, 4)));
                }
                else
                {
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleFrom(NPC.Center + new Vector2(0, 4)));
                }
                dust.noGravity = true;
                dust.velocity = DustSpeed;
                dust.position += NPC.velocity;
            }
        }

        public override Color? GetAlpha(Color drawColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Lighting.AddLight(NPC.Center, ExtendedColor.JellyOrange.ToVector3() * 0.5f);

            Asset<Texture2D> GlowBall = ModContent.GetTexture("Blockaroz14Mod/Assets/GlowBall_" + (short)1);

            Asset<Texture2D> BubbleTexture = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishBubble");
            Asset<Texture2D> CoreTexture = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishCore");
            Asset<Texture2D> CoreGlow = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishCore_Glow");

            float waveX = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.33f) * 0.075f;
            float waveY = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7.33f) * 0.075f;
            Vector2 wobble = new Vector2(1 + waveX, 1 + waveY);

            float rotation = NPC.rotation;
            Vector2 bubbleOrigin = BubbleTexture.Size() / 2;
            Vector2 coreOrigin = CoreGlow.Size() / 2;

            Vector2 trueScale = Vector2.One;
            Vector2 wobbleScale = trueScale;

            Color color = Color.White;
            color.A /= 2;

            if (DrawPhase == -1)//teleport
            {
                float t = (float)Math.Abs(Math.Cos(ExtraCounter / (TeleportTime / MathHelper.Pi)));

                trueScale = Vector2.One * Utils.GetLerpValue(0f, 1f, t * 1.1f, true);
                wobbleScale = Vector2.One * Utils.GetLerpValue(0f, 1f, t * 1.1f, true);

                float scaleFactor = Utils.GetLerpValue(1.5f, 0f, t * 1.1f, true) + 0.54f;
                bool dir = ExtraCounter > (TeleportTime / 2);
                DustHelper(dir, scaleFactor, BubbleDust);
            }
            else if (DrawPhase == -3)//more intense wobble
            {
                float waveX2 = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 16f) * 0.15f;
                float waveY2 = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 16f) * 0.15f;
                Vector2 wobble2 = new Vector2(1 + waveX2, 1 + waveY2);
                wobbleScale = wobble2 * trueScale;
            }
            else if (DrawPhase == 0)//default
            {
                wobbleScale = wobble * trueScale;
            }

            float waveW = (float)Math.Cos((Main.GlobalTimeWrappedHourly * 3.665f) + 1f) * 0.06f;
            Vector2 glowScale = (new Vector2(0.89f) + new Vector2(waveW)) * trueScale;
            Vector2 offsetVector = Vector2.UnitY * trueScale * 20;

            spriteBatch.Draw(CoreTexture.Value, NPC.Center - Main.screenPosition + offsetVector, null, Color.White, rotation, coreOrigin, trueScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(CoreGlow.Value, NPC.Center - Main.screenPosition + offsetVector, null, color * 0.5f, rotation, coreOrigin, glowScale, SpriteEffects.None, 0f);
            ExtendedUtils.DrawStreak(GlowBall, SpriteEffects.None, NPC.Center - Main.screenPosition + offsetVector, GlowBall.Size() / 2f, 1.5f, glowScale.X, glowScale.Y, rotation, ExtendedColor.JellyOrange, Color.Goldenrod);

            spriteBatch.Draw(BubbleTexture.Value, NPC.Center - Main.screenPosition, null, color, rotation, bubbleOrigin, wobbleScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}