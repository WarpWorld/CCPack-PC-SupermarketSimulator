using System;
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

namespace BepinControl
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestMod : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "WarpWorld.CrowdControl";
        private const string modName = "Crowd Control";
        private const string modVersion = "1.0.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource mls;

        internal static TestMod Instance = null;
        private ControlClient client = null;

        public static bool ForceUseCash = false;
        public static bool ForceUseCredit = false;
        public static bool ForceMath = false;


        void Awake()
        {


            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Crowd Control");

            mls.LogInfo($"Loaded {modGUID}. Patching.");
            harmony.PatchAll(typeof(TestMod));
            harmony.PatchAll();
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
