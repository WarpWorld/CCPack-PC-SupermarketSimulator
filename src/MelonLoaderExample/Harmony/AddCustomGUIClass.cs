using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(PlayerInteraction), "Update")]
public static class PlayerInteraction_Update
{
    static void Postfix(PlayerInteraction __instance) => CustomGUIMessages.Update();
}