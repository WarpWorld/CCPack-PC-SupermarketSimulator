using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(PlayerInteraction), "Start")]
public static class AddCustomGUIClass
{
    static void Postfix(PlayerInteraction __instance)
    {
        if (__instance.gameObject.GetComponent<CustomGUIMessages>() == null) __instance.gameObject.AddComponent<CustomGUIMessages>();
    }
}