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
            NPC.width = 162;
            NPC.height = 154;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath22;

            Music = MusicID.OtherworldlyBoss1;
            NPC.lifeMax = 7000;
            NPC.lifeRegen = 1;
            NPC.defense = 40;
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

        public static float PhaseTimerMax = 40;

        public static float TeleportTime = 40;

        public static float BaseAttackTime = 100;

        public static float TrailAttackTime = 20;

        public static float TrailAttackTimeMax = 140;

        public static int DustType = ModContent.DustType<JellyfishBubbleDust>();

        public static int CoreOffset = 50;

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

            if (OverallPhase < 3)
            {
                PhaseTimer++;
                if (PhaseTimer > PhaseTimerMax)
                    PhaseTimer = PhaseTimerMax;

                if (Phase <= 0)
                    Phase++;

                if (Phase == 1 && PhaseTimer == PhaseTimerMax && TeleportNext == false)
                    BaseAttack(player);

                else if (Phase == 1 && PhaseTimer == PhaseTimerMax && TeleportNext == true)
                    SpinExplosionAttack(player);

                else if (Phase == 2 && PhaseTimer == PhaseTimerMax)
                    PlayerTrailedAttack(player);

                else if (Phase == 3 && PhaseTimer == PhaseTimerMax && OverallPhase == 2)
                    ExplosionAttack(player);

                else if (TeleportNext == true)
                    Teleport(player);

                else
                {
                    DrawPhase = 0;
                    Movement(player.MountedCenter, 0.55f);
                }
            }
            else if (OverallPhase == 3)
            {
                PhaseTimer++;
                if (PhaseTimer >= 120)
                {
                    NPC.immortal = false;
                    DrawPhase = 0;
                    ResetAI(true);
                    PhaseTimer = 0;
                    OverallPhase = 4;
                }
                else
                {
                    DustHelper(true, 1.2f);
                    NPC.immortal = true;
                    DrawPhase = 1;
                }
            }
            else if (OverallPhase == 4)
            {
                PhaseTimer++;
                if (PhaseTimer > PhaseTimerMax)
                    PhaseTimer = PhaseTimerMax;
            }
        }

        public void PhaseCheck()
        {
            if (NPC.GetLifePercent() < 0.3f && PhaseTimer < 1)
                OverallPhase = 3;
            else if (NPC.GetLifePercent() < 0.6f)
                OverallPhase = 2;
            else
                OverallPhase = 1;
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

        public static int GetScaledNumbers(int count)
        {
            if (Main.expertMode)
                count += 15;

            if (Main.getGoodWorld)
                count += 15;

            return count;
        }

        public void Movement(Vector2 position, float speed = 1f, float rotationDegrees = 0)
        {
            if (Vector2.Distance(NPC.Center, position) > 50)
                NPC.velocity += NPC.DirectionTo(position).RotatedByRandom(MathHelper.ToRadians(rotationDegrees)) * speed;
            else
                SlowMovement(0.98f);

            if (NPC.velocity.Length() > 1.5f)
                NPC.velocity *= 0.933f;
        }

        public void SlowMovement(float harshness = 0.933f)
        {
            NPC.velocity *= harshness;
            if (NPC.velocity.Length() < 0.0044f)
                NPC.velocity = Vector2.Zero;
        }

        public static Vector2 GetPredictedPosition(Player player, float intensity) => player.Center + (player.velocity * 2f * intensity);
 

        public void Teleport(Player player)
        {
            DrawPhase = -1;

            SlowMovement();

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

        public void DashTo(Vector2 vector, float speed, bool instant = false)
        {
            DrawPhase = -2;

            NPC.knockBackResist = 0f;

            if (instant == false)
            {
                ExtraCounter++;
                if (ExtraCounter < 15)
                {
                    SlowMovement(0.75f);
                    NPC.velocity += NPC.DirectionTo(vector) * (speed / 5);
                    DustHelper(true, 1f);
                }
                if (ExtraCounter == 20)
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

            if (ExtraCounter >= 21)
            {
                ExtraCounter = 0;
                DrawPhase = 0;
                NPC.knockBackResist = 0.7f;
                return;
            }
        }

        public void BaseAttack(Player player)
        {
            Movement(player.MountedCenter, 0.33f);

            LocalCounter++;

            for (int i = 0; i < (BaseAttackTime / 10); i++)
            {
                if (LocalCounter == 10 * i && LocalCounter > 20)
                {
                    float rotation = NPC.Center.DirectionTo(GetPredictedPosition(player, 2f)).ToRotation();
                    Vector2 offset = new Vector2(CoreOffset, 0).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, NPC.Center.DirectionTo(player.MountedCenter) * (7f + i), ModContent.ProjectileType<JellyfishBoltProj>(), NPC.damage, 0);
                }
            }
            if (LocalCounter >= BaseAttackTime)
            {
                ResetAI(false);
                return;
            }
        }

        public void SpinExplosionAttack(Player player)
        {
            LocalCounter++;
            int TotalProjectiles = GetScaledNumbers(30);

            for (int i = 0; i < TotalProjectiles; i++)
            {
                if (LocalCounter == (i * 3))
                {
                    float rotation = ExtendedUtils.GetCircle(i * 3, TotalProjectiles) + (Main.rand.NextFloat(-3, 3) / 30);
                    Vector2 offset = new Vector2(0, CoreOffset).RotatedBy(rotation);
                    Vector2 speed = new Vector2(0, Main.rand.Next(4, 7)).RotatedBy(rotation);
                    Projectile.NewProjectile(NPC.Center + offset, speed, ModContent.ProjectileType<JellyfishBoltProj>(), NPC.damage, 0);
                }
            }

            if (LocalCounter > 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 1f);
            }
            else
                DrawPhase = -3;

            if (LocalCounter >= 60)
            {
                ResetAI(false);
                return;
            }
        }

        public void PlayerTrailedAttack(Player player)
        {
            LocalCounter++;
            if (LocalCounter > 15)
                Movement(player.MountedCenter, 0.15f);
            else
                SlowMovement();

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

                    if (LocalCounter == TrailAttackTime * multiplier)
                    {
                        Vector2 pos1 = ExtendedUtils.GetPositionAroundTarget(target.MountedCenter, Main.rand.Next(60, 720), true);
                        Projectile.NewProjectile(pos1, pos1.DirectionTo(target.MountedCenter), ModContent.ProjectileType<JellyfishExplosionProj>(), NPC.damage, 0);
                    }

                    if (LocalCounter == (TrailAttackTime * multiplier * 2f) + (TrailAttackTime * 0.5f) && OverallPhase == 2)
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

        public void ExplosionAttack(Player player)
        {
            int TotalExplosionProjectiles = GetScaledNumbers(10);

            DrawPhase = -3;

            SlowMovement(0.8f);

            LocalCounter++;

            if (LocalCounter < 25)
            {
                DustHelper(true, 1f);
            }
            if (LocalCounter == 30)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);

                for (int i = 0; i < TotalExplosionProjectiles; i++)
                {
                    DustHelper(false, 1f);
                    Vector2 speed = new Vector2(0, Main.rand.Next(7, 11)).RotatedBy(ExtendedUtils.GetCircle(i, TotalExplosionProjectiles)).RotatedByRandom(0.7f);
                    Projectile.NewProjectile(NPC.Center, speed, ModContent.ProjectileType<JellyfishLightningProj>(), NPC.damage, 0);
                }
            }

            if (LocalCounter >= 50)
            {
                DrawPhase = 0;
                Movement(player.MountedCenter, 0.7f);
            }
            if (LocalCounter >= 70)
            {
                if (Main.rand.Next(2) == 0 && OverallPhase == 2)
                    TeleportNext = true;

                ResetAI(true);
                return;
            }
        }

        public void DustHelper(bool inward, float scale)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Main.dust[Dust.NewDust(NPC.position - new Vector2(8, 8), NPC.width + 16, NPC.height + 16, DustType, 0, 0, 100, Color.White, scale)];
                dust.noGravity = true;
                Vector2 DustSpeed;
                if (inward)
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleTo(NPC.Center));
                else
                    DustSpeed = new Vector2(i + 3, 0).RotatedBy(dust.position.AngleFrom(NPC.Center));
                dust.velocity = DustSpeed;
                dust.position += NPC.velocity;
            }
        }

        public override Color? GetAlpha(Color drawColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Asset<Texture2D> BubbleTexture = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/JellyfishBubble");
            //Asset<Texture2D> CoreTexture = ModContent.GetTexture("Blockaroz14Mod/Content/NPCs/JellyfishBoss/p");

            float rotation = NPC.rotation;
            Vector2 origin = NPC.Size / 2;
            Vector2 scale = Vector2.One;
            Color color = Color.White;
            color.A /= 2;
            Color color2 = Color.White * 0.2f;
            color2.A = 0;

            float waveX = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 7.33f) * 0.065f;
            float waveY = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.33f) * 0.065f;
            Vector2 wobble = new Vector2(1 + waveX, 1 + waveY);

            if (DrawPhase == -1)//teleport
            {
                float t = (float)Math.Abs(Math.Cos(ExtraCounter / (TeleportTime / Math.PI)));

                scale = Vector2.One * Utils.GetLerpValue(0f, 1f, t * 1.11f, true);

                float scaleFactor = Utils.GetLerpValue(1.5f, 0f, t * 1.11f, true) + 0.54f;
                bool dir = ExtraCounter > (TeleportTime / 2);
                DustHelper(dir, scaleFactor);
            }
            else if (DrawPhase == -3 || DrawPhase == 1)//more intense wobble
            {
                float waveX2 = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 16f) * 0.12f;
                float waveY2 = (float)Math.Cos(Main.GlobalTimeWrappedHourly * 16f) * 0.12f;
                Vector2 wobble2 = new Vector2(1 + waveX2, 1 + waveY2);
                scale = wobble2;
            }
            else if (DrawPhase == 0)
            {
                scale = wobble;
            }

            spriteBatch.Draw(BubbleTexture.Value, NPC.Center - Main.screenPosition, null, color, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}