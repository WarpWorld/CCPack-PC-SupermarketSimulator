using System;
using HarmonyLib;
using Il2Cpp__Project__.Scripts.Computer.Management.Hiring_Tab;
using Il2Cpp;
using Object = UnityEngine.Object;

namespace CrowdControl;

[HarmonyPatch(typeof(HiringTab), "OnEnable")]
public static class HiringTab_OnEnable
{
    static void Prefix(HiringTab __instance)
    {
        try
        {
            foreach (RestockerItem restockerItem in Object.FindObjectsOfType<RestockerItem>())
                restockerItem.Start();
            
            foreach (var cashierItem in Object.FindObjectsOfType<CashierItem>())
                cashierItem.Start();

            foreach (var customerHelperItem in Object.FindObjectsOfType<CustomerHelperItem>())
                customerHelperItem.Start();
        }
        catch (Exception ex)
        {
            CrowdControlMod.Instance.Logger.Error($"Error in HiringTab OnEnable patch: {ex.Message}");
        }
    }
}