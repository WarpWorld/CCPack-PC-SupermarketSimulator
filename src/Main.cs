﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Security.AccessControl;
using BepInEx.Configuration;
using System.Reflection;
using Unity.Netcode;
using static System.Net.Mime.MediaTypeNames;
using Steamworks.Ugc;
using System.Threading;
using BepinControl;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using TMPro;
using UnityEngine.EventSystems;
using __Project__.Scripts.Computer.Management.Hiring_Tab;
using MyBox;
using System.Net.Sockets;



namespace BepinControl
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestMod : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "WarpWorld.CrowdControl";
        private const string modName = "Crowd Control";
        private const string modVersion = "1.4.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource mls;

        internal static TestMod Instance = null;
        private ControlClient client = null;

        public static bool isFocused = true;
        public static bool ForceUseCash = false;
        public static bool ForceUseCredit = false;
        public static bool ForceRequireChange = false;
        public static bool ForceMath = false;
        public static bool ForceExactChange = false;
        public static bool AllowMischarge = false;
        public static bool ForceLargeBills = false;
        public static int CurrentLanguage = 0;

        public static int OrgLanguage = 0;
        public static int NewLanguage = 0;


        public static string currentHeldItem;

        public static string NameOverride = "";
        public static List<GameObject> nameplates = new List<GameObject>();


        void Awake()
        {


            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Crowd Control");

            mls.LogInfo($"Loaded {modGUID}. Patching.");
            harmony.PatchAll(typeof(TestMod));
            harmony.PatchAll();
            CustomerGeneratorPatches.ApplyPatches(harmony);

            mls.LogInfo($"Initializing Crowd Control");

            try
            {
                client = new ControlClient();
                new Thread(new ThreadStart(client.NetworkLoop)).Start();
                new Thread(new ThreadStart(client.RequestLoop)).Start();
            }
            catch (Exception e)
            {
                mls.LogInfo($"CC Init Error: {e.ToString()}");
            }

            mls.LogInfo($"Crowd Control Initialized");


            mls = Logger;
        }


        public class CustomGUIMessages : MonoBehaviour
        {
            public enum Language
            {
                English = 0,
                French = 1,
                German = 2,
                Italian = 3,
                Spanish = 4,
                Portugal = 5,
                Brazil = 6,
                Netherlands = 7,
                Turkey = 8
            }

            private readonly Dictionary<string, Dictionary<Language, string>> flagMessages = new Dictionary<string, Dictionary<Language, string>>
            {
                {
                    "ForceUseCash", new Dictionary<Language, string>
                    {
                        { Language.English, "All customers only have cash." },
                        { Language.French, "Tous les clients n'ont que de l'argent liquide." },
                        { Language.German, "Alle Kunden haben nur Bargeld." },
                        { Language.Italian, "Tutti i clienti hanno solo contanti." },
                        { Language.Spanish, "Todos los clientes solo tienen efectivo." },
                        { Language.Portugal, "Todos os clientes só têm dinheiro." },
                        { Language.Brazil, "Todos os clientes só têm dinheiro." },
                        { Language.Netherlands, "Alle klanten hebben alleen contant geld." },
                        { Language.Turkey, "Tüm müşterilerin sadece nakit parası var." }
                    }
                },
                {
                    "ForceUseCredit", new Dictionary<Language, string>
                    {
                        { Language.English, "All customers only have card." },
                        { Language.French, "Tous les clients n'ont que des cartes." },
                        { Language.German, "Alle Kunden haben nur Karten." },
                        { Language.Italian, "Tutti i clienti hanno solo carta." },
                        { Language.Spanish, "Todos los clientes solo tienen tarjeta de credito." },
                        { Language.Portugal, "Todos os clientes só têm cartão." },
                        { Language.Brazil, "Todos os clientes só têm cartão." },
                        { Language.Netherlands, "Alle klanten hebben alleen een kaart." },
                        { Language.Turkey, "Tüm müşterilerin sadece kartı var." }
                    }
                },
                {
                    "ForceExactChange", new Dictionary<Language, string>
                    {
                        { Language.English, "All customers will pay in exact change." },
                        { Language.French, "Tous les clients paieront avec l'appoint exact." },
                        { Language.German, "Alle Kunden zahlen mit genauem Wechselgeld." },
                        { Language.Italian, "Tutti i clienti pagheranno con il resto esatto." },
                        { Language.Spanish, "Todos los clientes pagarán con el cambio exacto." },
                        { Language.Portugal, "Todos os clientes pagarão com o troco exato." },
                        { Language.Brazil, "Todos os clientes pagarão com o troco exato." },
                        { Language.Netherlands, "Alle klanten betalen met precies wisselgeld." },
                        { Language.Turkey, "Tüm müşteriler tam para üstüyle ödeyecek." }
                    }
                },
                {
                    "ForceRequireChange", new Dictionary<Language, string>
                    {
                        { Language.English, "All customers will not pay in exact change." },
                        { Language.French, "Tous les clients ne paieront pas avec l'appoint exact." },
                        { Language.German, "Alle Kunden werden nicht mit dem genauen Betrag bezahlen." },
                        { Language.Italian, "Tutti i clienti non pagheranno con il resto esatto." },
                        { Language.Spanish, "Todos los clientes no pagarán con el cambio exacto." },
                        { Language.Portugal, "Todos os clientes não pagarão com o troco exato." },
                        { Language.Brazil, "Todos os clientes não vão pagar com o troco exato." },
                        { Language.Netherlands, "Niet alle klanten zullen met het exacte wisselgeld betalen." },
                        { Language.Turkey, "Tüm müşteriler tam para üstü ile ödeme yapmayacak." }
                    }
                },
                {
                    "AllowMischarge", new Dictionary<Language, string>
                    {
                        { Language.English, "You can currently overcharge card payments." },
                        { Language.French, "Vous pouvez actuellement surcharger les paiements par carte." },
                        { Language.German, "Sie können derzeit Kartenzahlungen überladen." },
                        { Language.Italian, "Attualmente puoi addebitare eccessivamente i pagamenti con carta." },
                        { Language.Spanish, "Actualmente puedes sobrecargar los pagos con tarjeta de credito." },
                        { Language.Portugal, "Atualmente, você pode sobrecarregar os pagamentos com cartão." },
                        { Language.Brazil, "Atualmente, você pode sobrecarregar os pagamentos com cartão." },
                        { Language.Netherlands, "U kunt momenteel kaartbetalingen te veel in rekening brengen." },
                        { Language.Turkey, "Şu anda kart ödemelerinden fazla ücret alabilirsiniz." }
                    }
                }
            };

            private List<string> activeMessages = new List<string>();

            void Update()
            {
                UpdateActiveMessages();
            }

            void UpdateActiveMessages()
            {
                if (CurrentLanguage > 8) CurrentLanguage = 0;
                Language currentLanguage = (Language)CurrentLanguage;

                activeMessages.Clear();
                if (ForceUseCash) activeMessages.Add(flagMessages["ForceUseCash"][currentLanguage]);
                if (ForceUseCredit) activeMessages.Add(flagMessages["ForceUseCredit"][currentLanguage]);
                if (ForceExactChange) activeMessages.Add(flagMessages["ForceExactChange"][currentLanguage]);
                if (ForceRequireChange) activeMessages.Add(flagMessages["ForceRequireChange"][currentLanguage]);
                if (AllowMischarge) activeMessages.Add(flagMessages["AllowMischarge"][currentLanguage]);
            }

            void OnGUI()
            {
                GUIStyle guiStyle = new GUIStyle();
                guiStyle.fontSize = 14;

                int yOffset = 0; // Vertical offset for each message
                foreach (string message in activeMessages)
                {
                    GUI.Label(new Rect(10, 10 + yOffset, 300, 50), message, guiStyle);
                    yOffset += 20; // Increase the offset for the next message
                }
            }
        }


        public static Queue<Action> ActionQueue = new Queue<Action>();

        //attach this to some game class with a function that runs every frame like the player's Update()
        [HarmonyPatch(typeof(PlayerInteraction), "Update")]
        [HarmonyPrefix]
        static void RunEffects()
        {



            while (ActionQueue.Count > 0)
            {
                Action action = ActionQueue.Dequeue();
                action.Invoke();
            }

            lock (TimedThread.threads)
            {
                foreach (var thread in TimedThread.threads)
                {
                    if (!thread.paused)
                        thread.effect.tick();
                }
            }

        }

        public static class CustomerGeneratorPatches
        {
            public static void ApplyPatches(Harmony harmonyInstance)
            {
                var originalSpawn = typeof(CustomerGenerator).GetMethod("Spawn", new Type[] { });
                var postfixSpawn = new HarmonyMethod(typeof(CustomerGeneratorPatches).GetMethod(nameof(SpawnPostfix), BindingFlags.Static | BindingFlags.NonPublic));
                harmonyInstance.Patch(originalSpawn, null, postfixSpawn);

                var originalSpawnVector = typeof(CustomerGenerator).GetMethod("Spawn", new Type[] { typeof(Vector3) });
                var postfixSpawnVector = new HarmonyMethod(typeof(CustomerGeneratorPatches).GetMethod(nameof(SpawnVectorPostfix), BindingFlags.Static | BindingFlags.NonPublic));
                harmonyInstance.Patch(originalSpawnVector, null, postfixSpawnVector);
            }

            private static void SpawnPostfix(Customer __result)
            {
                AddNamePlateToCustomer(__result);
            }

            private static void SpawnVectorPostfix(Customer __result)
            {
                AddNamePlateToCustomer(__result);
            }

            private static void AddNamePlateToCustomer(Customer customer)
            {
                if (customer == null) return;



                if (customer.transform.Find("NamePlate") != null) return;

                //string chatName = CustomerChatNames.GetChatName(customer.gameObject.GetInstanceID());
                string chatName = NameOverride;



                if (string.IsNullOrEmpty(chatName)) return;


                NameOverride = "";

                GameObject namePlate = new GameObject("NamePlate");
                namePlate.transform.SetParent(customer.transform);
                namePlate.transform.localPosition = Vector3.up * 1.6f;
                namePlate.transform.LookAt(2 * namePlate.transform.position - Camera.main.transform.position);

                TextMeshPro tmp = namePlate.AddComponent<TextMeshPro>();
                nameplates.Add(namePlate);

                tmp.text = chatName;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontSize = 1;

                //need to make it always face the camera... at least would be nice

            }
        }


        [HarmonyPatch(typeof(PlayerInteraction), "SetCurrentInteraction")]
        public static class PlayerInteraction_SetCurrentInteraction_Patch
        {
            static void Postfix(InteractactableType type)
            {

                currentHeldItem = type.ToString();
                //mls.LogInfo($"{currentHeldItem}");
            }
        }


        [HarmonyPatch(typeof(EventSystem), "OnApplicationFocus")]
        public static class EventSystem_OnApplicationFocus_Patch
        {
            static void Postfix(bool hasFocus)
            {
                isFocused = hasFocus;
            }
        }


        [HarmonyPatch(typeof(PlayerInteraction), "Start")]
        public static class AddCustomGUIClassPatch
        {
            static void Postfix(PlayerInteraction __instance)
            {
                if (__instance.gameObject.GetComponent<CustomGUIMessages>() == null) __instance.gameObject.AddComponent<CustomGUIMessages>();
            }
        }


        [HarmonyPatch(typeof(Checkout), "TryFinishingCardPayment")]
        public static class TryFinishingCardPayment_Patch
        {
            public static bool Prefix(ref bool __result, ref float posTotal, Checkout __instance)
            {
                if (!AllowMischarge) return true;
                FieldInfo totalPriceField = AccessTools.Field(typeof(Checkout), "m_TotalPrice");
                if (((float)totalPriceField.GetValue(__instance)) * 1.5 >= posTotal) totalPriceField.SetValue(__instance, posTotal);

                return true;
            }

        }



        [HarmonyPatch(typeof(MoneyGenerator), "SpawnCustomerPayment")]
        public static class MoneyGenerator_SpawnCustomerPayment_Patch
        {
            static void Postfix(ref GameObject __result)
            {

                if (__result != null)
                {
                    if (ForceLargeBills)
                    {
                        float size = Random.Range(6.0f, 24.0f);
                        __result.transform.localScale = new Vector3(size, size, size);
                    }
                }
            }

        }


        [HarmonyPatch(typeof(CustomerPayment), "GenerateRandomPayment")]
        public static class CustomerPayment_GenerateRandomPayment_ExactChange_Patch
        {
            public static bool Prefix(ref float __result, float totalPrice)
            {
                if (ForceRequireChange)
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

                if (!ForceExactChange) return true;
                __result = totalPrice;
                return false;
            }
        }

        [HarmonyPatch(typeof(Customer), "DoPayment")]
        public static class Customer_DoPayment_Patch
        {
            public static void Prefix(ref bool viaCreditCard)
            {
                if (ForceUseCash)
                {
                    viaCreditCard = false;
                    return;
                }
                if (ForceUseCredit)
                {
                    viaCreditCard = true;
                    return;
                }

            }
        }


        [HarmonyPatch(typeof(CashRegisterScreen))] // Target the CashRegisterScreen class
        [HarmonyPatch("CorrectChangeText", MethodType.Setter)] // Target the setter of the CorrectChangeText property
        public static class CorrectChangeTextPatch
        {
            static bool Prefix(CashRegisterScreen __instance)
            {
                // Access the TMP_Text component directly and set its text
                TMP_Text textComponent = (TMP_Text)AccessTools.Field(__instance.GetType(), "m_CorrectChangeText").GetValue(__instance);
                if (textComponent != null && ForceMath)
                {
                    textComponent.text = "DO THE MATH";
                    return false;
                }

                return textComponent;
            }
        }

        [HarmonyPatch(typeof(MusicPlayerManager))]
        [HarmonyPatch("OnApplicationQuit")]
        public class OnApplicationQuitPatch
        {
            static bool Prefix()
            {

                try
                {
                    if (ControlClient.Socket != null && ControlClient.Socket.Connected)
                    {
                        ControlClient.Socket.Shutdown(SocketShutdown.Both);
                        ControlClient.Socket.Close();
                        ControlClient.Socket.Dispose();
                    }


                    ControlClient clientInstance = new ControlClient();
                    clientInstance.Stop();
                    mls.LogInfo("ControlClient stopped successfully.");
                }
                catch (Exception ex)
                {
                    mls.LogError($"Error during application quit: {ex}");
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(HiringTab), "OnEnable")]
        public static class HiringTabOnEnablePatch
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

        [HarmonyPatch(typeof(Checkout), "ChangeState")]
        public static class Checkout_ChangeState_Patch
        {

            public static void Prefix(ref Checkout.State newState)
            {

                if (ForceUseCredit || ForceUseCash)
                {
                    if (newState == Checkout.State.CUSTOMER_HANDING_CASH || newState == Checkout.State.CUSTOMER_HANDING_CARD)
                    {
                        if (ForceUseCredit) newState = Checkout.State.CUSTOMER_HANDING_CARD;
                        if (ForceUseCash) newState = Checkout.State.CUSTOMER_HANDING_CASH;
                    }

                    if (newState == Checkout.State.PAYMENT_CASH || newState == Checkout.State.PAYMENT_CREDIT_CARD)
                    {
                        if (ForceUseCash) newState = Checkout.State.PAYMENT_CASH;
                        if (ForceUseCredit) newState = Checkout.State.PAYMENT_CREDIT_CARD;
                    }
                }


            }
        }


    }

}
