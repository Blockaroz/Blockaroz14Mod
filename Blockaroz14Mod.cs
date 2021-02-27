using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Blockaroz14Mod
{
    public class Blockaroz14Mod : Mod
    {
        public override void Load()
        {
            LoadShaders();
        }

        public override void PostSetupContent()
        {
            //SoundEngine.PlaySound(SoundID.AchievementComplete);
        }

        public void LoadShaders()
        {
            Ref<Effect> vertexPixelShaderRef = Main.VertexPixelShaderRef;

            GameShaders.Misc["Warbler"] = new MiscShaderData(vertexPixelShaderRef, "MagicMissile").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["Warbler"].UseImage0("Images/Extra_" + (short)190);
            GameShaders.Misc["Warbler"].UseImage1("Images/Extra_" + (short)197);
            GameShaders.Misc["Warbler"].UseImage2("Images/Extra_" + (short)193);

            GameShaders.Misc["Nodachi"] = new MiscShaderData(vertexPixelShaderRef, "FinalFractalVertex").UseProjectionMatrix(doUse: true);
            GameShaders.Misc["Nodachi"].UseImage0("Images/Extra_" + (short)209);
            GameShaders.Misc["Nodachi"].UseImage1("Images/Misc/Noise");
            GameShaders.Misc["Nodachi"].UseImage2("Images/Extra_" + (short)193);
        }
    }
}