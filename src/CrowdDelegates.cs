using __Project__.Scripts.Computer.Management.Hiring_Tab;
using __Project__.Scripts.Data;
using __Project__.Scripts.Managers;
using BepInEx;
using DG.Tweening;
using Lean.Pool;
using MyBox;
using Newtonsoft.Json.Linq;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Pseudo;
using UnityEngine.UI;
using static SaveManager;
using static System.Collections.Specialized.BitVector32;
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
        public static int maxBoxCount = 100;
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

        private static readonly Dictionary<string, Dictionary<Language, string>> chatMessages = new Dictionary<string, Dictionary<Language, string>>
                {
                    {
                        "Pizza", new Dictionary<Language, string>
                        {
                            { Language.English, "Lemme get that pizza BONELESS!" },
                            { Language.French, "Laisse-moi avoir cette pizza SANS OS !" },
                            { Language.German, "Lass mir diese Pizza OHNE KNOCHEN!" },
                            { Language.Italian, "Dammi quella pizza SENZA OSSA!" },
                            { Language.Spanish, "¡Dame esa pizza SIN HUESOS!" },
                            { Language.Portugal, "Deixa-me ter essa pizza SEM OSSOS!" },
                            { Language.Brazil, "Deixa eu pegar essa pizza SEM OSSO!" },
                            { Language.Netherlands, "Laat me die pizza ZONDER BOTTEN hebben!" },
                            { Language.Turkey, "Bana kemiksiz o pizzayı ver!" }
                        }
                    },
                    {
                        "Soup", new Dictionary<Language, string>
                        {
                            { Language.English, "I'm at soup!" },
                            { Language.French, "Je suis à la soupe !" },
                            { Language.German, "Ich bin bei der Suppe!" },
                            { Language.Italian, "Sono alla zuppa!" },
                            { Language.Spanish, "¡Estoy en la sopa!" },
                            { Language.Portugal, "Estou na sopa!" },
                            { Language.Brazil, "Estou na sopa!" },
                            { Language.Netherlands, "Ik ben bij de soep!" },
                            { Language.Turkey, "Çorbadayım!" }
                        }
                    },
                    {
                        "Breakfast", new Dictionary<Language, string>
                        {
                            { Language.English, "It's breakfast time." },
                            { Language.French, "C'est l'heure du petit-déjeuner." },
                            { Language.German, "Es ist Frühstückszeit." },
                            { Language.Italian, "È ora di colazione." },
                            { Language.Spanish, "Es hora del desayuno." },
                            { Language.Portugal, "É hora do café da manhã." },
                            { Language.Brazil, "É hora do café da manhã." },
                            { Language.Netherlands, "Het is ontbijttijd." },
                            { Language.Turkey, "Kahvaltı zamanı." }
                        }
                    },
                    {
                        "Robbery", new Dictionary<Language, string>
                        {
                            { Language.English, "This is a robbery!" },
                            { Language.French, "C'est un vol !" },
                            { Language.German, "Das ist ein Überfall!" },
                            { Language.Italian, "Questa è una rapina!" },
                            { Language.Spanish, "¡Esto es un robo!" },
                            { Language.Portugal, "Isto é um assalto!" },
                            { Language.Brazil, "Isto é um assalto!" },
                            { Language.Netherlands, "Dit is een overval!" },
                            { Language.Turkey, "Bu bir soygun!" }
                        }
                    }
                };


        public static string GetChatMessage(string phrase)
        {

            GetCurrentLanguage();

            Language currentLanguage = (Language)TestMod.CurrentLanguage;
            if (currentLanguage < 0) currentLanguage = 0;
            if (chatMessages.TryGetValue(phrase, out var languageDict))
            {
                if (languageDict.TryGetValue(currentLanguage, out var message))
                {
                    return message;
                }
            }



            return "Message not found";

        }


        public AssetBundle bundle; // Make sure this is assigned when the plugin loads

        //v2022.3.38.12965287

        private static GameObject hypetrainPrefab;


        // Static flag to ensure assets are loaded only once
        private static bool loaded = false;

        // Load all assets from the bundle and store them
        public void LoadAssetsFromBundle()
        {
            if (loaded) return; // Only load once

            //TestMod.mls.LogDebug("PATH " + System.IO.Path.Combine(Paths.PluginPath, "CrowdControl", "food"));

                    



            HypeTrainBoxData boxData = new HypeTrainBoxData();// Do this to load the dll... maybe do something different, but this works for now
            bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Paths.PluginPath, "CrowdControl", "warpworld.hypetrain"));
            if (bundle == null)
            {
                Debug.LogError("Failed to load AssetBundle.");
                return;
            }

            hypetrainPrefab = bundle.LoadAsset<GameObject>("HypeTrain");

            if (hypetrainPrefab == null)
            {
                Debug.LogError("hypetrain prefab not found in AssetBundle.");
            }

            loaded = true; // Mark as loaded after successful loading
        }

        public static UnityEngine.Color ConvertUserNameToColor(string userName)
        {
            // Step 1: Hash the user name
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(userName));

                // Step 2: Use the first three bytes of the hash to create RGB values (normalized 0-1 for Unity's Color)
                float r = hashBytes[0] / 255f;
                float g = hashBytes[1] / 255f;
                float b = hashBytes[2] / 255f;

                // Step 3: Return a new Unity Color object
                return new UnityEngine.Color(r, g, b);
            }
        }

        public void Spawn_HypeTrain(Vector3 position, Quaternion rotation, CrowdRequest.SourceDetails sourceDetails)
        {
            if (hypetrainPrefab != null)
            {
                /*for (int i = 0; i < 32; ++i)
                {
                    try
                    {
                        Debug.Log($"Layer {i} is: {LayerMask.LayerToName(i)}");
						for (int j = 0; j < 32; ++j)
						{
							try
							{
                                if (i != j)
                                {
                                    Debug.Log($"Collide with  {LayerMask.LayerToName(j)}: {Physics.GetIgnoreLayerCollision(i, j)}");
                                }
							}
							catch { }
						}
					}
                    catch { }
                }*/

                if (sourceDetails.top_contributions.Length == 0)
                {
                    return;
                }

               

                HypeTrain hypeTrain = UnityEngine.Object.Instantiate(hypetrainPrefab, position, rotation).GetComponent<HypeTrain>();
                if (null == hypeTrain)
                {
                    Debug.LogError("SAAAAAAAAAAAD");
                }
                else
                {
                    Vector3 initialStartOffset = new Vector3(-14.5f, 0.2f, 6.0f); // Further away by 2 units
                    Vector3 initialStopOffset = new Vector3(14.5f, 0.2f, 6.0f); // Further away by 2 units

                    Transform playerCamera = Camera.main?.transform;

                    if (playerCamera == null)
                    {
                        playerCamera = UnityEngine.Object.FindObjectOfType<Camera>()?.transform;
                        if (playerCamera == null)
                        {
                            return;
                        }
                    }

                    PlayerInteraction player = Singleton<PlayerInteraction>.Instance;

                    Transform playerTransform = Singleton<PlayerController>.Instance.transform;


                    Vector3 startPos = playerTransform.position + playerCamera.TransformDirection(initialStartOffset);
                    startPos.y = playerTransform.position.y;

                    Vector3 stopPos = playerTransform.position + playerCamera.TransformDirection(initialStopOffset);
                    stopPos.y = playerTransform.position.y;

                    List<HypeTrainBoxData> hypeTrainBoxDataList = new List<HypeTrainBoxData>();

                    foreach (var contribution in sourceDetails.top_contributions)
                    {
                        hypeTrainBoxDataList.Add(new HypeTrainBoxData()
                        {
                            name = contribution.user_name,
                            box_color = ConvertUserNameToColor(contribution.user_name),
                            bit_amount = contribution.type == "bits" ? contribution.total : 0 // Only set bit_amount if the contribution is bits
                        });
                    }

                    bool isLastContributionInTop = sourceDetails.last_contribution != null;

                    // Only add last train car if the last_contribution user_id is not in top_contributions
                    if (isLastContributionInTop)
                    {
                        hypeTrainBoxDataList.Add(new HypeTrainBoxData()
                        {
                            name = sourceDetails.last_contribution.user_name,
                            box_color = ConvertUserNameToColor(sourceDetails.last_contribution.user_name),
                            bit_amount = sourceDetails.last_contribution.type == "bits" ? sourceDetails.last_contribution.total : 0
                        });
                    }


                    // Now call StartHypeTrain with the generated hypeTrainBoxDataList
                    hypeTrain.StartHypeTrain(startPos, stopPos, hypeTrainBoxDataList.ToArray(), playerTransform,
                    new HypeTrainOptions()
                    {
                        train_layer = LayerMask.NameToLayer("FloorLayer"),
                        max_bits_per_car = 100,
                        //volume = SoundManager.SFXVolume
                    });

                }
            }
            else
            {
                Debug.LogError("train prefab not loaded.");
            }
        }

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




                        MoneyManager MM = Singleton<MoneyManager>.Instance;
                        MM.MoneyTransition(100.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);
                        //MoneyTransition(100.0f, MoneyManager.TransitionType.CHECKOUT_INCOME);

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
                        MoneyManager MM = Singleton<MoneyManager>.Instance;
                        MM.MoneyTransition(1000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);

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
                        MoneyManager MM = Singleton<MoneyManager>.Instance; 
                        MM.MoneyTransition(10000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);

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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't remove more cash than they have");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        MoneyManager MM = Singleton<MoneyManager>.Instance;
                        MM.MoneyTransition(-100.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);

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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't remove more cash than they have");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        MoneyManager MM = Singleton<MoneyManager>.Instance;
                        MM.MoneyTransition(-1000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);

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
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't remove more cash than they have");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        MoneyManager MM = Singleton<MoneyManager>.Instance;
                        MM.MoneyTransition(-10000.0f, MoneyManager.TransitionType.CHECKOUT_INCOME, true);

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
            if (dm.CurrentHour >= 8 && !dm.AM) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

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
            if (dm.CurrentHour >= 9 && !dm.AM) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

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

            //GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
            //int boxCount = boxes.Length;
            //if (boxCount > maxBoxCount) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Too many boxes");

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

            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
     

            if (player.InInteraction && (TestMod.currentHeldItem == "COMPUTER" || TestMod.currentHeldItem == "CHECKOUT")) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {

                        Transform pos = Singleton<PlayerController>.Instance.transform;
                        //TestMod.mls.LogInfo($"Player POS: {pos.position}");

                        string location = req.code.Split('_')[1];
                        Vector3 teleportPosition = new Vector3();
                        switch (location)
                        {
                            case "outsidestore":
                                Transform loadingPOS = (Transform)getProperty(Singleton<DeliveryManager>.Instance, "m_DeliveryPosition");
                                teleportPosition = loadingPOS.position;
                                break;
                            case "acrossstreet":
                                teleportPosition = new Vector3(32.07f, -0.07f, 4.14f);
                                break;
                            case "computer":
                                //ComputerInteraction computer = Singleton<ComputerInteraction>.Instance;
                                //Transform computerPOS = Singleton<ComputerInteraction>.Instance.transform;
                                //Singleton<SaveManager>.Instance.Progression.ComputerTransform.Position
                                //teleportPosition = Singleton<SaveManager>.Instance.Progression.ComputerTransform.Position;

                                TransformData computerTransform = Singleton<SaveManager>.Instance.Progression.ComputerTransform;

                                Vector3 computerPos = computerTransform.Position;
                                Quaternion computerRot = computerTransform.Rotation;

                                // Direction the computer is facing
                                Vector3 forward = computerRot * Vector3.forward;

                                // Step behind the forward vector instead of in front
                                float offsetDistance = 1.0f;
                                teleportPosition = computerPos - forward * offsetDistance;

                                break;
                            case "faraway":
                                Vector3[] positions = new Vector3[]
                                {
                                    new Vector3(-64.75f, -0.04f, 46.34f),
                                    new Vector3(90.38f, -0.06f, -52.40f),
                                    //  new Vector3(98.13f, -0.01f, 64.64f),
                                    new Vector3(-65.27f, -0.06f, -24.99f)
                                };
                                System.Random random = new System.Random();
                                int randomIndex = random.Next(0, positions.Length);
                                teleportPosition = positions[randomIndex];
                                break;
                        }

                        //TestMod.mls.LogInfo($"teleportPosition: {teleportPosition}");
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

            //GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
            //int boxCount = boxes.Length;
            //if (boxCount > maxBoxCount) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Too many boxes");

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


        public static CrowdResponse ClearBoxes(ControlClient client, CrowdRequest req)
        {


            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";

            GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
            int boxCount = boxes.Length;
            //TestMod.mls.LogInfo($"Box Count: {boxCount}");

            if (boxCount < 1) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "No boxes to despawn.");

            try
            {
                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {

                        foreach (GameObject box in boxes)
                        {
                            Box boxComponent = box.GetComponent<Box>();
                            if (boxComponent) box.SetActive(false);
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

        public static CrowdResponse PlayerSendEmptyBox(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            //if (boxCount > maxBoxCount) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Too many boxes");
            //TestMod.mls.LogInfo($"boxCount: {boxCount}");
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
                        //box.Setup(-1, true);


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
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");


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





 


        public static CrowdResponse SpawnGarbage(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            try
            {
                GarbageManager garbageManager = GarbageManager.Instance;


                //if (!garbageManager.CanSpawnGarbage) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Unable to Spawn Garbage");

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        garbageManager.SpawnGarbage();
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

                //ignore that this is inInteraction, we can determine if im holding or not
                bool inInteraction = (bool)getProperty(player, "m_InInteraction");

                if (!inInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                if (!TestMod.currentHeldItem.Contains("BOX")) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<PlayerObjectHolder>.Instance.ThrowObject();
                        //player.onThrow();

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

                bool inInteraction = (bool)getProperty(player, "m_InInteraction");
                TestMod.mls.LogInfo($"DROP ITEM: {inInteraction} {TestMod.currentHeldItem}");
                if (!inInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);
                if (!TestMod.currentHeldItem.Contains("BOX")) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, message);

                TestMod.ActionQueue.Enqueue(() =>
                {
                    try
                    {
                        Singleton<PlayerObjectHolder>.Instance.DropObject();
                        //player.onDrop();
                        setProperty(player, "m_InInteraction", false);
                        setProperty(player, "m_PlacingMode", false);
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
                        //moved spawn from door to delivery position cause m_StoreDoor is no longer used?
                        //Transform door = (Transform)getProperty(Singleton<CustomerManager>.Instance, "m_StoreDoor");
                        Transform door = (Transform)getProperty(Singleton<DeliveryManager>.Instance, "m_DeliveryPosition");

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

            List<Customer> cust = (List<Customer>)getProperty(Singleton<CustomerManager>.Instance, "m_ActiveCustomers");
            bool despawned = false;

            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {
                    foreach (var c in cust)
                    {

                        if (!despawned)
                        {
                            Checkout check;
                            check = (Checkout)getProperty(c, "m_Checkout");
                            bool shop = (bool)getProperty(c, "m_StartedShopping");

                            if (shop && !check && !despawned)
                            {
                                despawned = true;
                                Singleton<CustomerGenerator>.Instance.DeSpawn(c);
                                status = CrowdResponse.Status.STATUS_SUCCESS;
                                break;
                            }

                            if (shop && check && !despawned)
                            {

                                var customersFieldInfo = typeof(Checkout).GetField("m_Customers", BindingFlags.Instance | BindingFlags.NonPublic);
                                List<Customer> customerList = (List<Customer>)customersFieldInfo.GetValue(check);

                                if (customerList != null)
                                {
                                    int firstCustomerID = customerList[0].GetInstanceID();
                                    if (firstCustomerID != c.GetInstanceID())
                                    {
                                        callFunc(c, "OnDisable", null);
                                        check.Unsubscribe(c);
                                        Singleton<CustomerGenerator>.Instance.DeSpawn(c);
                                        despawned = true;
                                        status = CrowdResponse.Status.STATUS_SUCCESS;
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Crowd Control Error: {e.ToString()}");
                    status = CrowdResponse.Status.STATUS_RETRY;
                }
            });

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


                                text.text = GetChatMessage("Robbery"); //chatMessages["Robbery"][currentLanguage];
                                //text.text = "This is a robbery!";
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

                                Language currentLanguage = (Language)TestMod.CurrentLanguage;

                                text.text = GetChatMessage("Soup"); //chatMessages["Soup"][currentLanguage];
                                //text.text = "I'm at Soup!";
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

                                Language currentLanguage = (Language)TestMod.CurrentLanguage;

                                text.text = GetChatMessage("Breakfast"); //chatMessages["Breakfast"][currentLanguage];
                                //text.text = "It's breakfast time!";
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

                                GetCurrentLanguage();

                                CustomerSpeech speechObject = LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, new string[] { });
                                DOVirtual.DelayedCall(time, delegate
                                {
                                    LeanPool.Despawn(speechObject, 0f);
                                }, true);

                                TMP_Text text = (TMP_Text)getProperty(speechObject, "m_SpeechText");


                                text.text = GetChatMessage("Pizza");// chatMessages["Pizza"][currentLanguage];
                                //text.text = "Lemme get that pizza BONELESS";
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
            //Section[] sections = (Section[])getProperty(Singleton<SectionManager>.Instance, "m_Sections");

            //TestMod.mls.LogInfo($"level: {level.ToString()}");
            //if (level >= sections.Length) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (level >= 22) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Store has reached max level.");
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

            //StorageSection[] sections = (StorageSection[])getProperty(Singleton<StorageSectionManager>.Instance, "m_StorageSections");

            if (!Singleton<SaveManager>.Instance.Storage.Purchased) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            //if (level >= sections.Length) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (level >= 14) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Storage has reached max level.");

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

            if (Singleton<EmployeeManager>.Instance.CashiersData.Count >= 6) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't hire any more cashiers.");


            if (!Singleton<SaveManager>.Instance.Storage.Purchased)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

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
            if (target.ID > 6) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't hire any more cashiers.");

            if (Singleton<EmployeeManager>.Instance.CashiersData.Contains(target.ID)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            CashierItem[] cashierItems = UnityEngine.Object.FindObjectsOfType<CashierItem>();
            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {

                    if (cashierItems.Length == 0)
                    {
                        Singleton<EmployeeManager>.Instance.HireCashier(target.ID, 0, out bool success);

                    }
                    else
                    {

                        foreach (var cashierItem in cashierItems)
                        {
                            var hiredProperty = cashierItem.GetType().GetProperty("Hired", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (hiredProperty != null)
                            {
                                if (!(bool)hiredProperty.GetValue(cashierItem))
                                {
                                    cashierItem.Hire();
                                    break;
                                }
                            }
                        }
                    }



                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during cashier hiring: {e.Message}");
                }
            });

            return new CrowdResponse(req.GetReqID(), status, message);

        }




        public static CrowdResponse FireCashier(ControlClient client, CrowdRequest req)
        {

            List<CashierSO> cashiers = (List<CashierSO>)getProperty(Singleton<IDManager>.Instance, "m_Cashiers");


            EmployeeManager employeeManager = Singleton<EmployeeManager>.Instance;

            int employeeCount = employeeManager.CashiersData?.Count ?? 0;

            if (employeeCount < 1)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "No cashiers to fire.");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            CashierItem[] cashierItems = UnityEngine.Object.FindObjectsOfType<CashierItem>();
            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {

                    Singleton<EmployeeManager>.Instance.FireCashier(Singleton<EmployeeManager>.Instance.CashiersData[0]);

                    if (cashierItems != null)
                    {
                        foreach (var cashierItem in cashierItems)
                        {
                            callFunc(cashierItem, "Start", null);
                        }
                    }

                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during cashier firing: {e.Message}");
                }
            });

            return new CrowdResponse(req.GetReqID(), status, message);

        }


        public static CrowdResponse HireCustomerHelper(ControlClient client, CrowdRequest req)
        {
            List<CustomerHelperSO> customerHelpers = (List<CustomerHelperSO>)getProperty(Singleton<IDManager>.Instance, "m_CustomerHelpers");

            if (Singleton<EmployeeManager>.Instance.ActiveCustomerHelpers.Count >= 2) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't hire any more customer helpers.");


            if (!Singleton<SaveManager>.Instance.Storage.Purchased)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            CustomerHelperSO target = null;
            foreach (CustomerHelperSO c in customerHelpers)
            {
                if (!Singleton<EmployeeManager>.Instance.CashiersData.Contains(c.ID))
                {
                    target = c;
                    break;
                }
            }


            if (target == null) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (target.ID > 2) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Can't hire any more customer helpers.");

            //if (Singleton<EmployeeManager>.Instance.ActiveCustomerHelpers.Contains(target.ID)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            CustomerHelperItem[] customerHelperItems = UnityEngine.Object.FindObjectsOfType<CustomerHelperItem>();

            TestMod.mls.LogInfo($"customerHelperItems: {customerHelperItems.Length}");


            foreach (var customerHelperItem in customerHelperItems)
            {
                TestMod.mls.LogInfo($"customerHelperItem: {customerHelperItem}");
            }

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {

                    if (customerHelperItems.Length == 0)
                    {
                        Singleton<EmployeeManager>.Instance.HireCustomerHelper(target.ID, 0, out bool success);
                    }
                    else
                    {

                        foreach (var customerHelperItem in customerHelperItems)
                        {
                            var hiredProperty = customerHelperItem.GetType().GetProperty("Hired", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (hiredProperty != null)
                            {
                                if (!(bool)hiredProperty.GetValue(customerHelperItem))
                                {
                                    customerHelperItem.Hire();
                                    break;
                                }
                            }
                        }
                    }



                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during customer helper hiring: {e.Message}");
                }
            });

            return new CrowdResponse(req.GetReqID(), status, message);

        }




        public static CrowdResponse FireCustomerHelper(ControlClient client, CrowdRequest req)
        {

            List<CustomerHelperSO> customerHelpers = (List<CustomerHelperSO>)getProperty(Singleton<IDManager>.Instance, "m_CustomerHelpers");


            EmployeeManager employeeManager = Singleton<EmployeeManager>.Instance;

            int employeeCount = employeeManager.ActiveCustomerHelpers?.Count ?? 0;

            if (employeeCount < 1)
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "No customer helpers to fire.");

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            CustomerHelperItem[] customerHelperItems = UnityEngine.Object.FindObjectsOfType<CustomerHelperItem>();
            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {

                    Singleton<EmployeeManager>.Instance.FireCustomerHelper(Singleton<EmployeeManager>.Instance.CashiersData[0]);

                    if (customerHelperItems != null)
                    {
                        foreach (var customerHelperItem in customerHelperItems)
                        {
                            callFunc(customerHelperItem, "Start", null);
                        }
                    }

                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during customer helper firing: {e.Message}");
                }
            });

            return new CrowdResponse(req.GetReqID(), status, message);

        }



        public static CrowdResponse HireRestocker(ControlClient client, CrowdRequest req)
        {


            if (!Singleton<SaveManager>.Instance.Storage.Purchased)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            // Get available restockers and owned IDs
            List<RestockerSO> restockers = (List<RestockerSO>)getProperty(Singleton<IDManager>.Instance, "m_Restockers");
            List<int> owned = (List<int>)getProperty(Singleton<EmployeeManager>.Instance, "m_RestockersData");

            // Find the first unowned restocker
            RestockerSO target = restockers.FirstOrDefault(c => !owned.Contains(c.ID));
            if (target == null)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            CrowdResponse.Status status = CrowdResponse.Status.STATUS_RETRY;
            string message = "";


            if (owned.Count >= 6 )
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = "Max restockers hired.";
                return new CrowdResponse(req.GetReqID(), status, message);
            }

            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {
                    RestockerItem[] restockerItems = UnityEngine.Object.FindObjectsOfType<RestockerItem>();

                    if (restockerItems.Length == 0)
                    {
                        Singleton<EmployeeManager>.Instance.HireRestocker(target.ID, 0);
                    }
                    else
                    {

                        foreach (var restockerItem in restockerItems)
                        {
                            var hiredProperty = restockerItem.GetType().GetProperty("Hired", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (hiredProperty != null)
                            {
                                if (!(bool)hiredProperty.GetValue(restockerItem))
                                {
                                    restockerItem.Hire();
                                    break;
                                }
                            }
                        }
                    }


                    
                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during restocker hiring: {e.Message}");
                }
            });

                status = CrowdResponse.Status.STATUS_SUCCESS;
                return new CrowdResponse(req.GetReqID(), status, message);
  
        }




        public static CrowdResponse FireRestocker(ControlClient client, CrowdRequest req)
        {


            if (!Singleton<SaveManager>.Instance.Storage.Purchased)
            {
                return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            }

            // Get available restockers and owned IDs
            List<RestockerSO> restockers = (List<RestockerSO>)getProperty(Singleton<IDManager>.Instance, "m_Restockers");
            List<int> owned = (List<int>)getProperty(Singleton<EmployeeManager>.Instance, "m_RestockersData");

  
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";


            if (owned.Count == 0 || owned == null)
            {
                status = CrowdResponse.Status.STATUS_FAILURE;
                message = "No Restockers to fire.";
                return new CrowdResponse(req.GetReqID(), status, message);
            }

            RestockerItem[] restockerItems = UnityEngine.Object.FindObjectsOfType<RestockerItem>();
            TestMod.ActionQueue.Enqueue(() =>
            {
                try
                {

                    Singleton<EmployeeManager>.Instance.FireRestocker(owned[0]);

                    if (restockerItems != null)
                    {
                        foreach (var restockerItem in restockerItems)
                        {
                            callFunc(restockerItem, "Start", null);
                        }
                    }

                }
                catch (Exception e)
                {
                    TestMod.mls.LogInfo($"Error during restocker firing: {e.Message}");
                }
            });

            return new CrowdResponse(req.GetReqID(), status, message);

        }





        public static CrowdResponse SpawnHypeTrain(ControlClient client, CrowdRequest req)
        {
            CrowdResponse.Status status = CrowdResponse.Status.STATUS_SUCCESS;
            string message = "";
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;

            Transform pos = Singleton<PlayerController>.Instance.transform;

            Vector3 position = pos.position;
            Quaternion rotation = pos.rotation;
            Transform playerCamera = Camera.main?.transform ?? UnityEngine.Object.FindObjectOfType<Camera>()?.transform;
            Vector3 forwardDirection = playerCamera.forward;

            if (!playerCamera) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_FAILURE, "Unable to spawn item.");


            TestMod.ActionQueue.Enqueue(() =>
            {

                CrowdDelegates crowdDelegatesInstance = new CrowdDelegates();

                crowdDelegatesInstance.LoadAssetsFromBundle();

                for (int i = 0; i < 1; i++)
                {
                    float spawnDifference = UnityEngine.Random.Range(0.1f, 1.0f);
                    Vector3 spawnPosition = new Vector3(
                        playerCamera.position.x + forwardDirection.x * spawnDifference,
                        playerCamera.position.y + 1.0f,
                        playerCamera.position.z + forwardDirection.z * spawnDifference
                    );


                    crowdDelegatesInstance.Spawn_HypeTrain(spawnPosition, Quaternion.identity, req.sourceDetails);

                }
            });

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
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.FORCE_MATH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CARD)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            CashRegisterScreen cashRegisterScreen = Singleton<CashRegisterScreen>.Instance;
            TMP_Text text = (TMP_Text)getProperty(cashRegisterScreen, "m_CorrectChangeText");
            text.text = "DO THE MATH";
            setProperty(cashRegisterScreen, "m_CorrectChangeText", text);

            new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_MATH, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse ForceRequireChange(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_REQUIRE_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CARD)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            GetCurrentLanguage();

            new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_REQUIRE_CHANGE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse ForceExactChange(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_REQUIRE_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
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

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.ALLOW_MISCHARGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_EXACT_CHANGE)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CASH)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            GetCurrentLanguage();


            new Thread(new TimedThread(req.GetReqID(), TimedType.ALLOW_MISCHARGE, dur * 1000).Run).Start();
            return new TimedResponse(req.GetReqID(), dur * 1000, CrowdResponse.Status.STATUS_SUCCESS);
        }


        public static CrowdResponse ForceLargeBills(ControlClient client, CrowdRequest req)
        {
            int dur = 30;
            if (req.duration > 0) dur = req.duration / 1000;
            TestMod.mls.LogInfo($"running");

            if (!Singleton<StoreStatus>.Instance.IsOpen) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            PlayerInteraction player = Singleton<PlayerInteraction>.Instance;
            if (!player.InInteraction) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TestMod.currentHeldItem != "CHECKOUT") return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            if (TimedThread.isRunning(TimedType.FORCE_LARGE_BILLS)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");
            if (TimedThread.isRunning(TimedType.FORCE_CARD)) return new CrowdResponse(req.GetReqID(), CrowdResponse.Status.STATUS_RETRY, "");

            new Thread(new TimedThread(req.GetReqID(), TimedType.FORCE_LARGE_BILLS, dur * 1000).Run).Start();
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
            if (currentLanguage > 8) TestMod.CurrentLanguage = 0;

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

            TestMod.NewLanguage = newLanguage;
            TestMod.OrgLanguage = currentLanguage;

            new Thread(new TimedThread(req.GetReqID(), TimedType.SET_LANGUAGE, dur * 1000).Run).Start();
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
