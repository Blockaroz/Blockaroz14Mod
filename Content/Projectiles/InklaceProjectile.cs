using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Projectiles
{
    public class InklaceProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            ProjectileID.Sets.IsAWhip[Projectile.type] = true;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 165;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
        }
    }
}