using HarmonyLib;
using Il2Cpp;
using Random = UnityEngine.Random;

namespace CrowdControl;

[HarmonyPatch(typeof(CustomerPayment), "GenerateRandomPayment")]
public static class CustomerPayment_GenerateRandomPayment_ExactChange
{
    public static bool Prefix(ref float __result, float totalPrice)
    {
        if (GameStateManager.ForceRequireChange)
        {

            float badLuck = Random.Range(0.0f, 1.0f);
            float randomChange = Random.Range(0.00f, 0.99f);

            if (badLuck < 0.1f)
            {
                __result = totalPrice + Random.Range(1, 100) + 1000 + randomChange;

            }
            else
            {
                __result = totalPrice + Random.Range(1, 100) + randomChange;
            }

            return false;

        }

        if (!GameStateManager.ForceExactChange) return true;
        __result = totalPrice;
        return false;
    }
}