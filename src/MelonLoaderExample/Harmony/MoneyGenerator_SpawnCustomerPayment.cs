using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CrowdControl;

[HarmonyPatch(typeof(MoneyGenerator), "SpawnCustomerPayment")]
public static class MoneyGenerator_SpawnCustomerPayment
{
    static void Postfix(ref GameObject __result)
    {

        if (__result != null)
        {
            if (GameStateManager.ForceLargeBills)
            {
                float size = Random.Range(6.0f, 24.0f);
                __result.transform.localScale = new Vector3(size, size, size);
            }
        }
    }

}