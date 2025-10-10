using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using Type = System.Type;

namespace CrowdControl;

public static class CustomerGeneratorPatches
{
    public static void ApplyPatches(HarmonyLib.Harmony harmonyInstance)
    {
        MethodInfo originalSpawn = typeof(CustomerGenerator).GetMethod("Spawn", new Type[] { });
        HarmonyMethod postfixSpawn = new(typeof(CustomerGeneratorPatches).GetMethod(nameof(SpawnPostfix), BindingFlags.Static | BindingFlags.NonPublic));
        harmonyInstance.Patch(originalSpawn, null, postfixSpawn);

        MethodInfo originalSpawnVector = typeof(CustomerGenerator).GetMethod("Spawn", new[] { typeof(Vector3) });
        HarmonyMethod postfixSpawnVector = new(typeof(CustomerGeneratorPatches).GetMethod(nameof(SpawnVectorPostfix), BindingFlags.Static | BindingFlags.NonPublic));
        harmonyInstance.Patch(originalSpawnVector, null, postfixSpawnVector);
    }

    private static void SpawnPostfix(Customer __result) => AddNamePlateToCustomer(__result);

    private static void SpawnVectorPostfix(Customer __result) => AddNamePlateToCustomer(__result);

    private static void AddNamePlateToCustomer(Customer customer)
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