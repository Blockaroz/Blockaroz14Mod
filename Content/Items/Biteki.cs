using Blockaroz14Mod.Content.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Blockaroz14Mod.Content.Items
{
    public class Biteki : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 10;
            Item.width = 42;
            Item.height = 42;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ModContent.RarityType<LightRainbowRarity>();
            //Item.shoot = ProjectileID.GladiusStab;
            //Item.shootSpeed = 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EnchantedSword)
                .AddIngredient(ItemID.FallenStar, 20)
                .AddCondition(Recipe.Condition.NearWater)
                .Register();
        }
    }
}