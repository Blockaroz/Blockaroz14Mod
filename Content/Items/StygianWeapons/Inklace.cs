using Blockaroz14Mod.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Items.StygianWeapons
{
    public class Inklace : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Whips are hardcoded wtf");
        }

        public override void SetDefaults()
        {
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.width = 18;
            Item.height = 18;
            Item.shoot = ModContent.ProjectileType<InklaceProjectile>();
            Item.UseSound = SoundID.Item152;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.noUseGraphic = true;
            Item.damage = 90;
            Item.knockBack = 15;
            //Item.shootSpeed = shootspeed;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.Register();
        }
    }
}