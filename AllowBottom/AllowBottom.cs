using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using AIChara;
using CharaCustom;
using BepInEx;
//using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;

namespace AllowBottom
{
    [BepInPlugin(GUID, GAME + " " + "Allow Bottom", VERSION)]
    [BepInProcess(GAMEPROCRESS)]
    [BepInProcess(STUDIOPROCESS)]
    public partial class AllowBottom : BaseUnityPlugin
    {
        public const string VERSION = "0.1.0";
        //public new static ManualLogSource Logger;
        public static ConfigEntry<bool> alwaysAllowBot { get; set; }

        private void Awake()
        {
            //Logger = base.Logger;
            var harmony = new Harmony(nameof(AllowBottom));
            harmony.PatchAll(typeof(AllowBottom));

            var iteratorType = typeof(AIChara.ChaControl).GetNestedType(TYPENAME, AccessTools.all);
            var iteratorMethod = AccessTools.Method(iteratorType, "MoveNext");
            var harmonyMethod = new HarmonyMethod(typeof(AllowBottom), nameof(AllowBottomChange));
            harmony.Patch(iteratorMethod, transpiler: harmonyMethod);

            alwaysAllowBot = Config.Bind("Setting", "Always allow bottom", false, new ConfigDescription("When enabled, bottom clothing is always allowed regardless of top."));
        }

        //Set notBot flag to false regardless of KeyType.Coordinate value for the current top
        [HarmonyPatch(typeof(AIChara.ChaControl), nameof(AIChara.ChaControl.ChangeAlphaMask2))]
        [HarmonyPatch(typeof(AIChara.ChaControl), nameof(AIChara.ChaControl.SetClothesState))]
        [HarmonyPatch(typeof(AIChara.ChaControl), nameof(AIChara.ChaControl.UpdateVisible))]
        [HarmonyPrefix]
        public static void yesBottom(AIChara.ChaControl __instance)
        {
            if (alwaysAllowBot.Value) __instance.notBot = false;
        }

        //Enable bottom and inner bottom sub menus in Character Creator regardless of KeyType.Coordinate value for the current top
        [HarmonyPatch(typeof(CharaCustom.CustomBase), nameof(CharaCustom.CustomBase.RestrictSubMenu))]
        [HarmonyPostfix]
        public static void EnableBotSubMenus(CharaCustom.CustomBase __instance)
        {
            if (alwaysAllowBot.Value) __instance.subMenuBot.interactable = __instance.subMenuInnerDown.interactable = true;
        }

        //Allow changing bottom regardless of KeyType.Coordinate value for the current top
        static IEnumerable<CodeInstruction> AllowBottomChange(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool flagFound = false;
            for (var i = 0; i < codes.Count; i++)
            {
                if (flagFound) break;
                if (codes[i].opcode == OpCodes.Ldc_I4_S && codes[i].operand.ToString() == "28") //find KeyType.Coordinate in order to locate the desired flag
                {
                    for (var j = i; j < codes.Count; j++)
                    {
                        if (codes[j].opcode == OpCodes.Ldc_I4_0) //find flag = false;
                        {
                            codes[j].opcode = OpCodes.Ldc_I4_1;
                            flagFound = true;
                            break;
                        }
                    }
                }
            }
            return codes.AsEnumerable();
        }
    }
}
