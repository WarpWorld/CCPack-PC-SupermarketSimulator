using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;

namespace CrowdControl;

[HarmonyPatch(typeof(CashRegisterScreen))] // Target the CashRegisterScreen class
[HarmonyPatch("CorrectChangeText", MethodType.Setter)] // Target the setter of the CorrectChangeText property
public static class CorrectChangeText
{
    static bool Prefix(CashRegisterScreen __instance)
    {
        // Access the TMP_Text component directly and set its text
        TMP_Text textComponent = (TMP_Text)AccessTools.Field(__instance.GetType(), "m_CorrectChangeText").GetValue(__instance);
        if (textComponent != null && GameStateManager.ForceMath)
        {
            textComponent.text = "DO THE MATH";
            return false;
        }

        return textComponent;
    }
}