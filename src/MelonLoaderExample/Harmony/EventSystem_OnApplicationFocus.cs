using HarmonyLib;
using UnityEngine.EventSystems;

namespace CrowdControl;

[HarmonyPatch(typeof(EventSystem), "OnApplicationFocus")]
public static class EventSystem_OnApplicationFocus
{
    static void Postfix(bool hasFocus)
    {
        GameStateManager.isFocused = hasFocus;
    }
}