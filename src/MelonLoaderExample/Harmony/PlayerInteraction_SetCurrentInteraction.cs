using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(PlayerInteraction), "SetCurrentInteraction")]
public static class PlayerInteraction_SetCurrentInteraction
{
    static void Postfix(InteractactableType type)
    {
        GameStateManager.currentHeldItem = type.ToString();
        //mls.LogInfo($"{currentHeldItem}");
    }
}