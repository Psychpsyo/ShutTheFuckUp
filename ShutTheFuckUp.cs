using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace ShutTheFuckUp {
    public class ShutTheFuckUp : NeosMod {
        public override string Name => "ShutTheFuckUp";
        public override string Author => "Psychpsyo";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/Psychpsyo/ShutTheFuckUp";


        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> modActive = new ModConfigurationKey<bool>("Active", "You can enable or disable the mod here.", () => true);

        private static ModConfiguration config;
        public override void OnEngineInit() {
            config = GetConfiguration();

            Harmony harmony = new Harmony("Psychpsyo.ShutTheFuckUp");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(AudioOutput), "IsUserExluded")]
        class AudioClipSilencer {
            static bool Prefix(AudioOutput __instance, ref bool __result, User user) {
                // do basic checks first:
                if (config.GetValue(modActive) &&
                    user == __instance.World.LocalUser) {
                    // now check if we're under a context menu (a slot virtual parented to "Radial Menu")
                    Slot checkSlot = __instance.Slot;
                    for (int i = 0; i < 6 && checkSlot != null; i++) {
                        VirtualParent suspect = checkSlot.GetComponent<VirtualParent>();
                        if (suspect != null && suspect?.OverrideParent?.RawTarget.Name == "Radial Menu") {
                            __result = true;
                            return false;
                        }
                        checkSlot = checkSlot.Parent;
                    }
                }

                // we're not under a context menu so don't skip the function
                return true;
            }
        }
    }
}
