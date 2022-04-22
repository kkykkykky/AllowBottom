using BepInEx;

namespace AllowBottom
{
    public partial class AllowBottom : BaseUnityPlugin
    {
        public const string GUID = "kky.ai.allowbottom";
        public const string GAME = "AI";
        public const string GAMEPROCRESS = "AI-Syoujyo";
        public const string STUDIOPROCESS = "StudioNEOV2";
        private static readonly string[] typeNames = { "<ChangeClothesBotAsync>c__IteratorA", "<ChangeClothesInnerBAsync>c__IteratorC" };
    }
}