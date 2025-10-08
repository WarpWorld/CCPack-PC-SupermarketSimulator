using HarmonyLib;
using Il2Cpp;

namespace CrowdControl;

[HarmonyPatch(typeof(Checkout), "ChangeState")]
public static class Checkout_ChangeState
{

    public static void Prefix(ref Checkout.State newState)
    {

        if (GameStateManager.ForceUseCredit || GameStateManager.ForceUseCash)
        {
            if (newState == Checkout.State.CUSTOMER_HANDING_CASH || newState == Checkout.State.CUSTOMER_HANDING_CARD)
            {
                if (GameStateManager.ForceUseCredit) newState = Checkout.State.CUSTOMER_HANDING_CARD;
                if (GameStateManager.ForceUseCash) newState = Checkout.State.CUSTOMER_HANDING_CASH;
            }

            if (newState == Checkout.State.PAYMENT_CASH || newState == Checkout.State.PAYMENT_CREDIT_CARD)
            {
                if (GameStateManager.ForceUseCash) newState = Checkout.State.PAYMENT_CASH;
                if (GameStateManager.ForceUseCredit) newState = Checkout.State.PAYMENT_CREDIT_CARD;
            }
        }


    }
}