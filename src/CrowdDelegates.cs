﻿using DG.Tweening;
using Lean.Pool;
using MyBox;
using Newtonsoft.Json.Linq;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization.Components;
using static SaveManager;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;




namespace BepinControl
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public static class CustomerChatNames
    {
        private static Dictionary<int, string> chatNames = new Dictionary<int, string>();

        public static void SetChatName(int customerId, string name)
        {
            chatNames[customerId] = name;
        }

        public static string GetChatName(int customerId)
        {
            if (chatNames.TryGetValue(customerId, out string name))
            {
                return name;
            }
            return null; // Or a default name
        }
    }



    public class CrowdDelegates
    {
        public static System.Random rnd = new System.Random();



        public static CrowdResponse Money100(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(100.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                        //Time.timeScale = 10.0f;

                        //Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, Singleton<PlayerController>.Instance.transform, new string[] { });


                        /*
                        CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");

                        TestMod.mls.LogInfo($"prefab: {prefab}");


                        LocalizationEntry localizationEntry = new LocalizationEntry();
                        localizationEntry.TableEntry = "Billy Mays here for OxyClean!";

                        Transform parent = Singleton<PlayerController>.Instance.transform;

                        CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, parent, false);
                        speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });

                        LocalizeStringEvent ev = (LocalizeStringEvent)getProperty(speechObject, "m_LocalizeStringEvent");

                        setProperty(ev.StringReference, "m_CurrentStringChangedValue", localizationEntry.TableEntry);
                        setProperty(speechObject, "m_LocalizeStringEvent", ev);

                        /*DOVirtual.DelayedCall(10.0f, delegate
                        {
                            LeanPool.Despawn(speechObject, 0f);
                        }, true);*/

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Money1000(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(1000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Money10000(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(10000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse MoneyN100(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (!Singleton<MoneyManager>.Instance.HasMoney(100.0f))
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(-100.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse MoneyN1000(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (!Singleton<MoneyManager>.Instance.HasMoney(1000.0f))
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(-1000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse MoneyN10000(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (!Singleton<MoneyManager>.Instance.HasMoney(10000.0f))
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<MoneyManager>.Instance.MoneyTransition(-10000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse PlusHour(ControlClient client, CrowdRequest req)
        {
            DayCycleManager dm = Singleton<DayCycleManager>.Instance;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (dm.CurrentHour >= 11 && !dm.AM) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        float h = (float)getProperty(dm, "m_DayDurationInGameTimeInSeconds");
                        //h /= 24.0f;

                        //float s = (float)getProperty(dm, "m_GameTimeScale");



                        float t = (float)getProperty(dm, "m_CurrentTimeInFloat");
                        t += 3600.0f;
                        setProperty(dm, "m_CurrentTimeInFloat", t);


                        t = Singleton<SaveManager>.Instance.Progression.CurrentTime;
                        t += 3600.0f;
                        Singleton<SaveManager>.Instance.Progression.CurrentTime = t;

                        setProperty(dm, "m_DayPercentage", t / h);

                        callFunc(dm, "UpdateGameTime", null);
                        callFunc(dm, "UpdateLighting", null);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse MinusHour(ControlClient client, CrowdRequest req)
        {
            DayCycleManager dm = Singleton<DayCycleManager>.Instance;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (dm.CurrentHour < 9 && dm.AM) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        float h = (float)getProperty(dm, "m_DayDurationInGameTimeInSeconds");

                        float t = Singleton<SaveManager>.Instance.Progression.CurrentTime;
                        t -= 3600.0f;
                        Singleton<SaveManager>.Instance.Progression.CurrentTime = t;

                        setProperty(dm, "m_DayPercentage", t / h);

                        int hour = (int)getProperty(dm, "m_CurrentTimeInHours");

                        if (hour == 1)
                        {
                            hour = 12;
                        }
                        else if (hour == 12)
                        {
                            hour = 11;
                            setProperty(dm, "m_AM", true);
                        }
                        else
                        {
                            hour--;
                        }

                        setProperty(dm, "m_CurrentTimeInHours", hour);


                        Action onFullHour = dm.OnFullHour;
                        if (onFullHour != null)
                        {
                            onFullHour();
                        }


                        Action onTimeChanged = dm.OnTimeChanged;
                        if (onTimeChanged != null)
                            onTimeChanged();


                        //callFunc(dm, "UpdateGameTime", null);
                        callFunc(dm, "UpdateLighting", null);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse OpenStore(ControlClient client, CrowdRequest req)
        {
            DayCycleManager dm = Singleton<DayCycleManager>.Instance;

            if (Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<StoreStatus>.Instance.IsOpen = true;

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse CloseStore(ControlClient client, CrowdRequest req)
        {
            DayCycleManager dm = Singleton<DayCycleManager>.Instance;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<StoreStatus>.Instance.IsOpen = false;

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static ProductSO getProduct(string name)
        {
            foreach (int productID in Singleton<ProductLicenseManager>.Instance.AllPoducts)
            {
                ProductSO p = Singleton<IDManager>.Instance.ProductSO(productID);

                if (p.ProductName.ToUpper().Contains(name.ToUpper())) return p;
            }
            return null;
        }

        public static CrowdResponse SendBox(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Transform pos = (Transform)getProperty(Singleton<DeliveryManager>.Instance, "m_DeliveryPosition");
                        string prod = req.code.Split('_')[1];
                        ProductSO product = getProduct(prod);
                        Box box = Singleton<BoxGenerator>.Instance.SpawnBox(product, pos.position + Vector3.up * Singleton<DeliveryManager>.Instance.space * (float)1.0f, Quaternion.identity, pos);
                        box.Setup(product.ID, true);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse TeleportPlayer(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {

                        Transform pos = Singleton<PlayerController>.Instance.transform;
                        TestMod.mls.LogInfo($"Player POS: {pos.position}");

                        string location = req.code.Split('_')[1];
                        Vector3 teleportPosition = new Vector3();
                        switch (location)
                        {
                            case "outsidestore":
                                Transform loadingPOS = (Transform)getProperty(Singleton<DeliveryManager>.Instance, "m_DeliveryPosition");
                                teleportPosition = loadingPOS.position;
                                break;
                            case "acrossstreet":
                                teleportPosition = new Vector3(15.80f, -0.06f, 6.22f);
                                break;
                            case "computer":
                                Transform computerPOS = (Transform)getProperty(Singleton<Computer>.Instance, "m_PlayerPosition");
                                teleportPosition = computerPOS.position;
                                break;
                            case "faraway":
                                Vector3[] positions = new Vector3[]
                                {
                                    new Vector3(-64.75f, -0.04f, 46.34f),
                                    new Vector3(90.38f, -0.06f, -52.40f),
                                    new Vector3(98.13f, -0.01f, 64.64f),
                                    new Vector3(-65.27f, -0.06f, -24.99f)
                                };
                                System.Random random = new System.Random();
                                int randomIndex = random.Next(0, positions.Length);
                                teleportPosition = positions[randomIndex];
                                break;
                        }

                        pos.position = teleportPosition;
                             
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);

        }




        public static CrowdResponse PlayerSendBox(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Transform pos = Singleton<PlayerController>.Instance.transform;

                        string prod = req.code.Split('_')[1];
                        ProductSO product = getProduct(prod);

                        Box box = Singleton<BoxGenerator>.Instance.SpawnBox(product, pos.position + Vector3.up * Singleton<DeliveryManager>.Instance.space * (float)2.0f, Quaternion.identity, null);
                        box.Setup(product.ID, true);


                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }



        public static CrowdResponse PlayerSendEmptyBox(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Transform pos = Singleton<PlayerController>.Instance.transform;

                        string prod = req.code.Split('_')[1];
                        ProductSO product = getProduct(prod);

                        Box box = Singleton<BoxGenerator>.Instance.SpawnBox(product, pos.position + Vector3.up * Singleton<DeliveryManager>.Instance.space * (float)2.0f, Quaternion.identity, null);
                        //-1 will make it empty, but will still use the products size from above
                        box.Setup(-1, true);


                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse ForcePaymentType(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

            bool found = false;
            foreach (var c in cust)
            {
                bool isHandingPayment = (bool)getProperty(c, "m_HandingPayment");
                if (isHandingPayment) found = true;
            }

            if (found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);


            if (TimedThread.isRunning(TimedType.FORCE_CASH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CARD)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            
            GetCurrentLanguage();
            
            string paymentType = req.code.Split('_')[1];

            if (paymentType == "cash") new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_CASH, dur * 1000).Run).Start();
            if (paymentType == "card") new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_CARD, dur * 1000).Run).Start();

            return new CrowdResponse(req.GetReqID(), status, message);
        }






        public static CrowdResponse ThrowItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


                try
            {
                PlayerObjectHolder hold = Singleton<PlayerObjectHolder>.Instance;
                PlayerInteraction player = Singleton<PlayerInteraction>.Instance;

                GameObject obj = (GameObject)getProperty(hold, "m_CurrentObject");

                if (obj == null)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }

                IPlacingMode mode = (IPlacingMode)getProperty(hold, "m_CurrentPlacingMode");

                if (mode == null && !obj.TryGetComponent<IPlacingMode>(out mode))
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }

                if (!mode.AvailablePosition)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }

                Rigidbody body;

                if (!obj.TryGetComponent<Rigidbody>(out body))
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }


                Throwable throwable;
                if (!obj.TryGetComponent<Throwable>(out throwable))
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<PlayerObjectHolder>.Instance.ThrowObject();
                        player.onThrow();
                        
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }

            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DropItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                PlayerObjectHolder hold = Singleton<PlayerObjectHolder>.Instance;
                PlayerInteraction player = Singleton<PlayerInteraction>.Instance;


                


                GameObject obj = (GameObject)getProperty(hold, "m_CurrentObject");

                if (obj == null)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }

                IPlacingMode mode = (IPlacingMode)getProperty(hold, "m_CurrentPlacingMode");

                if (mode == null && !obj.TryGetComponent<IPlacingMode>(out mode))
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }

                if (!mode.AvailablePosition)
                {
                    return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                }


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<PlayerObjectHolder>.Instance.DropObject();
                        player.onDrop();


                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });

            }

            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpawnCustomer(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Transform door = (Transform)getProperty(Singleton<CustomerManager>.Instance, "m_StoreDoor");

                        TestMod.NameOverride = req.viewer;
                        Customer customer = Singleton<CustomerGenerator>.Instance.Spawn(door.position);

                        

                        //CustomerChatNames.SetChatName(customer.gameObject.GetInstanceID(), req.viewer);
                        //TestMod.mls.LogInfo($"set name: {customer.gameObject.GetInstanceID()}");
                        customer.GoToStore(door.position);

                        List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");
                        
                        cust.Add(customer);
                        //customer.SetChatName("jaku");
                        setProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers", cust);

                        //Singleton<CustomerManager>.Instance.SpawnCustomer(door.position);


                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse DespawnCustomer(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {

                    bool isHandingPayment = (bool)getProperty(c, "m_HandingPayment");
                    bool isPayingViaCard = (bool)getProperty(c, "m_PaymentViaCreditCard");

                    Checkout checkingOut;
                    checkingOut = (Checkout)getProperty(c, "m_Checkout");
                    bool isCheckingOut = checkingOut ? true : false;

                    bool isInCheckout = (bool)getProperty(c, "m_InCheckout");
                    if (isInCheckout && !isCheckingOut && !isHandingPayment && !isPayingViaCard)
                    {
                        found = true;
                        break;
                    }

                    bool isShopping = (bool)getProperty(c, "m_StartedShopping");

                    if (isShopping && !isCheckingOut && !isHandingPayment && !isPayingViaCard)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {

                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            check = (Checkout)getProperty(c, "m_Checkout");

                            if (shop && !check)
                            {
                                //if (check) check.Unsubscribe(c);
                                Singleton<CustomerGenerator>.Instance.DeSpawn(c);
                                return;
                            }
                            shop = (bool)getProperty(c, "m_InCheckout");
                            if (shop && !check)
                            {
                                //if (check) check.Unsubscribe(c);
                                Singleton<CustomerGenerator>.Instance.DeSpawn(c);
                                return;
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Theft(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {
                    bool shop = (bool)getProperty(c, "m_StartedShopping");
                    if (shop)
                    {
                        found = true;
                        break;
                    }
                    shop = (bool)getProperty(c, "m_InCheckout");
                    if (shop)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {
                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            if (!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Robbery(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {
                    bool shop = (bool)getProperty(c, "m_StartedShopping");
                    if (shop)
                    {
                        found = true;
                        break;
                    }
                    shop = (bool)getProperty(c, "m_InCheckout");
                    if (shop)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {
                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            if (!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                //Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });

                                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                                if (localizationEntry == null)
                                {
                                    return;
                                }
                                CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");
                                float time = (float)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechLifetime");

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");
                                //setProperty(text, "m_text", "This is a robbery!");
                                text.text = "This is a robbery!";
                                setProperty(speechObject, "m_SpeechText", text);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse Soup(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {
                    bool shop = (bool)getProperty(c, "m_StartedShopping");
                    if (shop)
                    {
                        found = true;
                        break;
                    }
                    shop = (bool)getProperty(c, "m_InCheckout");
                    if (shop)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {
                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            if (!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                //Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });

                                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                                if (localizationEntry == null)
                                {
                                    return;
                                }
                                CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");
                                float time = (float)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechLifetime");

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");
                                //setProperty(text, "m_text", "This is a robbery!");
                                text.text = "I'm at Soup!";
                                setProperty(speechObject, "m_SpeechText", text);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Breakfast(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {
                    bool shop = (bool)getProperty(c, "m_StartedShopping");
                    if (shop)
                    {
                        found = true;
                        break;
                    }
                    shop = (bool)getProperty(c, "m_InCheckout");
                    if (shop)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {
                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            if (!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                //Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });

                                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                                if (localizationEntry == null)
                                {
                                    return;
                                }
                                CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");
                                float time = (float)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechLifetime");

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");
                                //setProperty(text, "m_text", "This is a robbery!");
                                text.text = "It's breakfast time!";
                                setProperty(speechObject, "m_SpeechText", text);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse Boneless(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {

                List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");

                bool found = false;
                foreach (var c in cust)
                {
                    bool shop = (bool)getProperty(c, "m_StartedShopping");
                    if (shop)
                    {
                        found = true;
                        break;
                    }
                    shop = (bool)getProperty(c, "m_InCheckout");
                    if (shop)
                    {
                        found = true;
                        break;
                    }

                }

                if (!found) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var c in cust)
                        {
                            Checkout check;
                            bool shop = (bool)getProperty(c, "m_StartedShopping");
                            if (!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                //Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });

                                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                                if (localizationEntry == null)
                                {
                                    return;
                                }
                                CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");
                                float time = (float)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechLifetime");

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");
                                //setProperty(text, "m_text", "This is a robbery!");
                                text.text = "Lemme get that pizza BONELESS";
                                setProperty(speechObject, "m_SpeechText", text);
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse LightsOn(ControlClient client, CrowdRequest req)
        {

            if (Singleton<StoreLightManager>.Instance.TurnOn) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<StoreLightManager>.Instance.TurnOn = true;
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse LightsOff(ControlClient client, CrowdRequest req)
        {

            if (!Singleton<StoreLightManager>.Instance.TurnOn) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<StoreLightManager>.Instance.TurnOn = false;
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }


        public static CrowdResponse UpgradeStore(ControlClient client, CrowdRequest req)
        {
            int level = Singleton<SaveManager>.Instance.Progression.StoreUpgradeLevel;
            Section[] sections = (Section[])getProperty(Singleton<SectionManager>.Instance, "m_Sections");

            if (level >= sections.Length) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<SectionManager>.Instance.UpgradeStore();
                        callFunc(Singleton<SectionManager>.Instance, "PlayAnimations", null);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse UpgradeStorage(ControlClient client, CrowdRequest req)
        {
            int level = Singleton<SaveManager>.Instance.Storage.StorageLevel;
            StorageSection[] sections = (StorageSection[])getProperty(Singleton<StorageSectionManager>.Instance, "m_StorageSections");

            if (!Singleton<SaveManager>.Instance.Storage.Purchased) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (level >= sections.Length) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<StorageSectionManager>.Instance.UpgradeStore();
                        callFunc(Singleton<StorageSectionManager>.Instance, "PlayAnimations", null);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HireCashier(ControlClient client, CrowdRequest req)
        {
            List<CashierSO> cashiers = (List<CashierSO>)getProperty(Singleton<IDManager>.Instance, "m_Cashiers");



            CashierSO target = null;
            foreach (CashierSO c in cashiers)
            {
                if (!Singleton<EmployeeManager>.Instance.CashiersData.Contains(c.ID))
                {
                    target = c;
                    break;
                }
            }

            if (target == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<EmployeeManager>.Instance.HireCashier(target.ID, 0);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse FireCashier(ControlClient client, CrowdRequest req)
        {
            List<CashierSO> cashiers = (List<CashierSO>)getProperty(Singleton<IDManager>.Instance, "m_Cashiers");




            if (Singleton<EmployeeManager>.Instance.CashiersData.Count == 0)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<EmployeeManager>.Instance.FireCashier(Singleton<EmployeeManager>.Instance.CashiersData[0]);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse HireRestocker(ControlClient client, CrowdRequest req)
        {
            if (!Singleton<SaveManager>.Instance.Storage.Purchased) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            List<RestockerSO> restockers = (List<RestockerSO>)getProperty(Singleton<IDManager>.Instance, "m_Restockers");

            List<int> owned = (List<int>)getProperty(Singleton<EmployeeManager>.Instance, "m_RestockersData");

            RestockerSO target = null;
            foreach (RestockerSO c in restockers)
            {
                if (!owned.Contains(c.ID))
                {
                    target = c;
                    break;
                }
            }

            if (target == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<EmployeeManager>.Instance.HireRestocker(target.ID, 0);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse FireRestocker(ControlClient client, CrowdRequest req)
        {
            List<int> owned = (List<int>)getProperty(Singleton<EmployeeManager>.Instance, "m_RestockersData");


            if (owned.Count == 0)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<EmployeeManager>.Instance.FireRestocker(owned[0]);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RaisePrices(ControlClient client, CrowdRequest req)
        {
            List<Pricing> prices = (List<Pricing>)getProperty(Singleton<PriceManager>.Instance, "m_PricesSetByPlayer");

            if (prices.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var price in prices)
                        {
                            price.Price *= 1.1f;
                            Singleton<PriceManager>.Instance.PriceSet(price);
                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse LowerPrices(ControlClient client, CrowdRequest req)
        {
            List<Pricing> prices = (List<Pricing>)getProperty(Singleton<PriceManager>.Instance, "m_PricesSetByPlayer");

            if (prices.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        foreach (var price in prices)
                        {
                            price.Price *= 0.8f;
                            if (price.Price < 0.25f) price.Price = 0.25f;
                            Singleton<PriceManager>.Instance.PriceSet(price);
                        }
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse LowerPrice(ControlClient client, CrowdRequest req)
        {
            List<Pricing> prices = (List<Pricing>)getProperty(Singleton<PriceManager>.Instance, "m_PricesSetByPlayer");

            if (prices.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            List<Pricing> target = new List<Pricing>();
            foreach (var price in prices)
            {
                if (price.Price > 0.25) target.Add(price);
            }


            if (target.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        int r = rnd.Next(target.Count);

                        Pricing price = target[r];

                        price.Price *= 0.8f;
                        if (price.Price < 0.25f) price.Price = 0.25f;
                        Singleton<PriceManager>.Instance.PriceSet(price);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RaisePrice(ControlClient client, CrowdRequest req)
        {
            List<Pricing> target = (List<Pricing>)getProperty(Singleton<PriceManager>.Instance, "m_PricesSetByPlayer");

            if (target.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        int r = rnd.Next(target.Count);

                        Pricing price = target[r];

                        price.Price *= 1.1f;
                        Singleton<PriceManager>.Instance.PriceSet(price);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse RemoveItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                List<Display> displays = (List<Display>)getProperty(Singleton<DisplayManager>.Instance, "m_Displays");

                TestMod.mls.LogInfo($"displays: {displays.Count}");

                List<DisplaySlot> target = new List<DisplaySlot>();

                foreach (var display in displays)
                {
                    DisplaySlot[] slots = (DisplaySlot[])getProperty(display, "m_DisplaySlots");

                    TestMod.mls.LogInfo($"slots: {slots.Length}");

                    foreach (var slot in slots)
                    {
                        ItemQuantity data = (ItemQuantity)getProperty(slot, "m_ProductCountData");

                        if (data == null || !data.HasProduct || data.FirstItemCount <= 0)
                        {

                        }
                        else
                        {
                            target.Add(slot);
                        }
                    }
                }

                if (target.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        int r = rnd.Next(target.Count);

                        DisplaySlot slot = target[r];

                        Product prod = slot.TakeProductFromDisplay();
                        LeanPool.Despawn(prod, 0f);

                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse AddItem(ControlClient client, CrowdRequest req)
        {

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                List<Display> displays = (List<Display>)getProperty(Singleton<DisplayManager>.Instance, "m_Displays");

                TestMod.mls.LogInfo($"displays: {displays.Count}");

                List<DisplaySlot> target = new List<DisplaySlot>();

                foreach (var display in displays)
                {
                    DisplaySlot[] slots = (DisplaySlot[])getProperty(display, "m_DisplaySlots");

                    TestMod.mls.LogInfo($"slots: {slots.Length}");

                    foreach (var slot in slots)
                    {
                        ItemQuantity data = (ItemQuantity)getProperty(slot, "m_ProductCountData");

                        if (data == null || !data.HasLabel || slot.Full)
                        {

                        }
                        else
                        {
                            target.Add(slot);
                        }
                    }
                }

                if (target.Count == 0) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        int r = rnd.Next(target.Count);

                        DisplaySlot slot = target[r];
                        ItemQuantity data = (ItemQuantity)getProperty(slot, "m_ProductCountData");

                        ProductSO productSO = Singleton<IDManager>.Instance.ProductSO(data.FirstItemID);

                        Product product = LeanPool.Spawn<Product>(productSO.ProductPrefab, slot.transform, false);
                        slot.AddProduct(data.FirstItemID, product);
                    }
                    catch (Exception e)
                    {
                        TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    }
                });
            }
            catch (Exception e)
            {
                status = CrowdResponse.Status.STATUS_RETRY;
                TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
            }

            return new CrowdResponse(req.GetReqID(), status, message);
        }

        public static CrowdResponse SpeedUltraSlow(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.GAME_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.GAME_ULTRA_SLOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SpeedSlow(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.GAME_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.GAME_SLOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SpeedFast(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.GAME_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.GAME_FAST, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SpeedUltraFast(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.GAME_ULTRA_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_SLOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.GAME_ULTRA_FAST)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.GAME_ULTRA_FAST, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse ForceMath(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_MATH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CashRegisterScreen cashRegisterScreen = Singleton<CashRegisterScreen>.Instance;
            TMP_Text text = (TMP_Text)getProperty(cashRegisterScreen, "m_CorrectChangeText");
            text.text = "DO THE MATH";
            setProperty(cashRegisterScreen, "m_CorrectChangeText", text);

            new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_MATH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }



        public static CrowdResponse ForceExactChange(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_MATH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CARD)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            GetCurrentLanguage();

            new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_EXACT_CHANGE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse AllowMischarges(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.ALLOW_MISCHARGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CASH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            GetCurrentLanguage();


            new Thread(new TimedThread(req.GetReqID(), TimedType.ALLOW_MISCHARGE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse InvertX(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;
            TestMod.mls.LogInfo($"running");

            if (TimedThread.isRunning(TimedType.INVERT_X)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.INVERT_X, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse InvertY(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.INVERT_Y)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.INVERT_Y, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SensitivityLow(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;


            if (TimedThread.isRunning(TimedType.SENSITIVITY_LOW)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.SENSITIVITY_LOW, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static CrowdResponse SensitivityHigh(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;


            if (TimedThread.isRunning(TimedType.SENSITIVITY_HIGH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.SENSITIVITY_HIGH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse HighFOV(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;


            if (TimedThread.isRunning(TimedType.HIGH_FOV)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.LOW_FOV)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.HIGH_FOV, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse LowFOV(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (TimedThread.isRunning(TimedType.HIGH_FOV)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.LOW_FOV)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.LOW_FOV, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }

        public static void GetCurrentLanguage()
        {

            // if this is running, we dont want to do anything
            if (TimedThread.isRunning(TimedType.SET_LANGUAGE)) return;

            SaveManager saveManager = Singleton<SaveManager>.Instance;
            int currentLanguage = saveManager.Settings.LanguageSetting;
            TestMod.CurrentLanguage = currentLanguage;
            return;

        }

        public static CrowdResponse SetLanguage(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            SaveManager saveManager = Singleton<SaveManager>.Instance;
            int currentLanguage = saveManager.Settings.LanguageSetting;

            string language = req.code.Split('_')[1];
            int newLanguage = 0;
            switch (language)
            {
                case "english":
                    {
                        newLanguage = 0;
                        break;
                    }
                case "french":
                    {
                        newLanguage = 1;
                        break;
                    }
                case "german":
                    {
                        newLanguage = 2;
                        break;
                    }
                case "italiano":
                    {
                        newLanguage = 3;
                        break;
                    }
                case "espanol":
                    {
                        newLanguage = 4;
                        break;
                    }
                case "portugal":
                    {
                        newLanguage = 5;
                        break;
                    }
                case "brazil":
                    {
                        newLanguage = 6;
                        break;
                    }
                case "nederlands":
                    {
                        newLanguage = 7;
                        break;
                    }
                case "turkce":
                    {
                        newLanguage = 8;
                        break;
                    }
            };


            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            if (currentLanguage == newLanguage) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "");
            if (TimedThread.isRunning(TimedType.SET_LANGUAGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            Dictionary<string, object> customVariables = new Dictionary<string, object>
            {
                { "oldLanguage", currentLanguage },
                { "newLanguage", newLanguage }
            };


            new Thread(new TimedThread(req.GetReqID(), TimedType.SET_LANGUAGE, dur * 1000, customVariables).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);

        }



        public static void setProperty(System.Object a, string prop, System.Object val)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            f.SetValue(a, val);
        }

        public static System.Object getProperty(System.Object a, string prop)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            return f.GetValue(a);
        }

        public static void setSubProperty(System.Object a, string prop, string prop2, System.Object val)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            var f2 = f.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);
            f2.SetValue(f, val);
        }

        public static void callSubFunc(System.Object a, string prop, string func, System.Object val)
        {
            callSubFunc(a, prop, func, new object[] { val });
        }

        public static void callSubFunc(System.Object a, string prop, string func, System.Object[] vals)
        {
            var f = a.GetType().GetField(prop, BindingFlags.Instance | BindingFlags.NonPublic);


            var p = f.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            p.Invoke(f, vals);

        }

        public static void callFunc(System.Object a, string func, System.Object val)
        {
            callFunc(a, func, new object[] { val });
        }

        public static void callFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            p.Invoke(a, vals);

        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object val)
        {
            return callAndReturnFunc(a, func, new object[] { val });
        }

        public static System.Object callAndReturnFunc(System.Object a, string func, System.Object[] vals)
        {
            var p = a.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.NonPublic);
            return p.Invoke(a, vals);

        }

    }
}
