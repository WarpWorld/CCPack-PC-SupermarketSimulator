using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(Customer), "DoPayment")]
public static class Customer_DoPayment
{
    public static void Prefix(ref bool viaCreditCard)
    {
        if (GameStateManager.ForceUseCash)
        {
            viaCreditCard = false;
            return;
        }
        if (GameStateManager.ForceUseCredit)
        {
            viaCreditCard = true;
            return;
        }

    }
}