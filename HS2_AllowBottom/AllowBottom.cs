using BepInEx;

namespace AllowBottom
{
    public partial class AllowBottom : BaseUnityPlugin
    {
        public const string GUID = "kky.hs2.allowbottom";
        public const string GAME = "HS2";
        public const string GAMEPROCRESS = "HoneySelect2";
        public const string STUDIOPROCESS = "StudioNEOV2";
        private static readonly string[] typeNames = { "<ChangeClothesBotAsync>d__490", "<ChangeClothesInnerBAsync>d__494" };
    }
}