using DG.Tweening;
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
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;


namespace BepinControl
{
    public delegate CrowdResponse CrowdDelegate(ControlClient client, CrowdRequest req);

    public class CrowdDelegates
    {
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

                            Time.timeScale = 10.0f;

                            Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, Singleton<PlayerController>.Instance.transform, new string[] { });
                            

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

            if(!Singleton<MoneyManager>.Instance.HasMoney(100.0f))
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
            if (dm.CurrentHour>=11 && !dm.AM) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

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
                        
                        if(hour == 1)
                        {
                            hour = 12;
                        } else if(hour == 12)
                        {
                            hour = 11;
                            setProperty(dm, "m_AM", true);
                        } else
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

        public static CrowdResponse ThrowItem(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            try
            {
                PlayerObjectHolder hold = Singleton<PlayerObjectHolder>.Instance;

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

                        Customer customer = Singleton<CustomerGenerator>.Instance.Spawn(door.position);
                        customer.GoToStore(door.position);
                        List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");
                        cust.Add(customer);
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
                            if (shop)
                            {
                                check = (Checkout)getProperty(c, "m_Checkout");
                                if (check) check.Unsubscribe(c);
                                Singleton<CustomerGenerator>.Instance.DeSpawn(c);
                                return;
                            }
                            shop = (bool)getProperty(c, "m_InCheckout");
                            if (shop)
                            {
                                check = (Checkout)getProperty(c, "m_Checkout");
                                if (check) check.Unsubscribe(c);
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
                            if(!shop) shop = (bool)getProperty(c, "m_InCheckout");

                            if (shop)
                            {
                                Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, new string[] { });

                                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                                if (localizationEntry == null)
                                {
                                    return;
                                }
                                CustomerSpeech prefab = (CustomerSpeech)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechPrefab");
                                float time = (float)getProperty(Singleton<WarningSystem>.Instance, "m_CustomerSpeechLifetime");

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                //speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");
                                //setProperty(text, "m_text", "This is a robbery!");
                                text.text = "This is a robbery!";
                                //setProperty(speechObject, "m_SpeechText", text);
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

        public static CrowdResponse SpeedUltraSlow(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if(!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

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
