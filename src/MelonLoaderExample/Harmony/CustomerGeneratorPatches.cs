using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using Type = System.Type;

namespace CrowdControl;

internal static class CustomerGeneratorPatches
{
    public static void AddNamePlateToCustomer(Customer customer)
    {
        if (customer == null) return;
        if (customer.transform.Find("NamePlate") != null) return;

        //string chatName = CustomerChatNames.GetChatName(customer.gameObject.GetInstanceID());
        string chatName = GameStateManager.NameOverride;
        
        if (string.IsNullOrEmpty(chatName)) return;
        
        GameStateManager.NameOverride = "";

        GameObject namePlate = new("NamePlate");
        namePlate.transform.SetParent(customer.transform);
        namePlate.transform.localPosition = Vector3.up * 1.6f;
        namePlate.transform.LookAt(2 * namePlate.transform.position - Camera.main.transform.position);

        TextMeshPro tmp = namePlate.AddComponent<TextMeshPro>();
        GameStateManager.nameplates.Add(namePlate);

        tmp.text = chatName;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 1;

        //need to make it always face the camera... at least would be nice
    }
}

[HarmonyPatch(typeof(CustomerGenerator), "Spawn", typeof(bool))]
public static class CustomerGenerator_Spawn_Patch
{
    public static void Prefix(Customer __instance) => CustomerGeneratorPatches.AddNamePlateToCustomer(__instance);
}

[HarmonyPatch(typeof(CustomerGenerator), "Spawn", typeof(Vector3))]
public static class CustomerGenerator_Spawn_Vector_Patch
{
    public static void Prefix(Customer __instance) => CustomerGeneratorPatches.AddNamePlateToCustomer(__instance);
}