using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;

namespace CrowdControl;

[HarmonyPatch(typeof(CashRegisterScreen), "CorrectChangeText", MethodType.Setter)]
public static class CashRegisterScreen_CorrectChangeText
{
    static bool Prefix(CashRegisterScreen __instance)
    {
        TMP_Text textComponent = __instance.m_CorrectChangeText;
        if (textComponent != null && GameStateManager.ForceMath)
        {
            textComponent.text = "DO THE MATH";
            return false;
        }

        return textComponent;
    }
}