using Blockaroz14Mod.Content.Projectiles;
using Blockaroz14Mod.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Items
{
    public class Nodachi : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 150;
            Item.width = 74;
            Item.height = 78;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item71;
            Item.rare = ModContent.RarityType<DarkRedRarity>();
            Item.shoot = ModContent.ProjectileType<NodachiSlash>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
            Item.knockBack = 30;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 center = player.MountedCenter;
            Vector2 distance = Main.MouseWorld - player.MountedCenter;

            if (distance.Length() > 190)
                distance = new Vector2(190, 0).RotatedBy(Utils.AngleTo(player.MountedCenter, Main.MouseWorld));

            distance += Main.rand.NextVector2Circular(15f, 15f);
            float circle = Main.rand.Next(-50, 50);
            Projectile.NewProjectile(center, distance / 2, type, damage, knockBack, player.whoAmI, circle);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}