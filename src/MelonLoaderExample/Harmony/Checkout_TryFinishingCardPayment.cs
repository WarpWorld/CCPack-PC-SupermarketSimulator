using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(Checkout), "TryFinishingCardPayment")]
public static class Checkout_TryFinishingCardPayment
{
    static bool Prefix(ref bool __result, ref float posTotal, Checkout __instance)
    {
        if (!GameStateManager.AllowMischarge) return true;

        if (__instance.TotalPrice * 1.5f >= posTotal)
            __instance.TotalPrice = posTotal;

        return true;
    }
}