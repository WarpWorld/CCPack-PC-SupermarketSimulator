using System.Reflection;
using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(Checkout), "TryFinishingCardPayment")]
public static class TryFinishingCardPayment
{
    public static bool Prefix(ref bool __result, ref float posTotal, Checkout __instance)
    {
        if (!GameStateManager.AllowMischarge) return true;
        FieldInfo totalPriceField = AccessTools.Field(typeof(Checkout), "m_TotalPrice");
        if (((float)totalPriceField.GetValue(__instance)) * 1.5 >= posTotal) totalPriceField.SetValue(__instance, posTotal);

        return true;
    }

}