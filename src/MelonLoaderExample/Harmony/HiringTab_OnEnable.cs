using HarmonyLib;
using Il2Cpp__Project__.Scripts.Computer.Management.Hiring_Tab;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(HiringTab), "OnEnable")]
public static class HiringTab_OnEnable
{
    static void Prefix(HiringTab __instance)
    {
        try
        {


            RestockerItem[] restockerItems = UnityEngine.Object.FindObjectsOfType<RestockerItem>();

            foreach (var restockerItem in restockerItems)
            {
                CrowdDelegates.callFunc(restockerItem, "Start", null);
            }

            CashierItem[] cashierItems = UnityEngine.Object.FindObjectsOfType<CashierItem>();

            foreach (var cashierItem in cashierItems)
            {
                CrowdDelegates.callFunc(cashierItem, "Start", null);
            }

            CustomerHelperItem[] customerHelperItems = UnityEngine.Object.FindObjectsOfType<CustomerHelperItem>();

            foreach (var customerHelperItem in customerHelperItems)
            {
                CrowdDelegates.callFunc(customerHelperItem, "Start", null);
            }


        }
        catch (Exception ex)
        {
            TestMod.mls.LogError($"Error in HiringTab OnEnable patch: {ex.Message}");
        }
    }
}