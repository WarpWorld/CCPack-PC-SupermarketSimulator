using System;
using System.Collections.Generic;
using System.Reflection;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppDG.Tweening;
using Il2CppLean.Pool;
using Il2CppTMPro;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("spawn", "despawn", "theft", "robbery", "soup", "breakfast", "boneless")]
public class CustomerEffects : Effect
{
    public CustomerEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private enum Language
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

    private static readonly Dictionary<string, Dictionary<Language, string>> chatMessages = new()
    {
        {
            "Pizza",
            new()
            {
                { Language.English, "Lemme get that pizza BONELESS!" }, { Language.French, "Laisse-moi avoir cette pizza SANS OS !" },
                { Language.German, "Lass mir diese Pizza OHNE KNOCHEN!" }, { Language.Italian, "Dammi quella pizza SENZA OSSA!" },
                { Language.Spanish, "¡Dame esa pizza SIN HUESOS!" }, { Language.Portugal, "Deixa-me ter essa pizza SEM OSSOS!" },
                { Language.Brazil, "Deixa eu pegar essa pizza SEM OSSO!" }, { Language.Netherlands, "Laat me die pizza ZONDER BOTTEN hebben!" },
                { Language.Turkey, "Bana kemiksiz o pizzayı ver!" }
            }
        },
        {
            "Soup",
            new()
            {
                { Language.English, "I'm at soup!" }, { Language.French, "Je suis à la soupe !" }, { Language.German, "Ich bin bei der Suppe!" },
                { Language.Italian, "Sono alla zuppa!" }, { Language.Spanish, "¡Estoy en la sopa!" }, { Language.Portugal, "Estou na sopa!" },
                { Language.Brazil, "Estou na sopa!" }, { Language.Netherlands, "Ik ben bij de soep!" }, { Language.Turkey, "Çorbadayım!" }
            }
        },
        {
            "Breakfast",
            new()
            {
                { Language.English, "It's breakfast time." }, { Language.French, "C'est l'heure du petit-déjeuner." }, { Language.German, "Es ist Frühstückszeit." },
                { Language.Italian, "È ora di colazione." }, { Language.Spanish, "Es hora del desayuno." }, { Language.Portugal, "É hora do café da manhã." },
                { Language.Brazil, "É hora do café da manhã." }, { Language.Netherlands, "Het is ontbijttijd." }, { Language.Turkey, "Kahvaltı zamanı." }
            }
        },
        {
            "Robbery",
            new()
            {
                { Language.English, "This is a robbery!" }, { Language.French, "C'est un vol !" }, { Language.German, "Das ist ein Überfall!" },
                { Language.Italian, "Questa è una rapina!" }, { Language.Spanish, "¡Esto es un robo!" }, { Language.Portugal, "Isto é um assalto!" },
                { Language.Brazil, "Isto é um assalto!" }, { Language.Netherlands, "Dit is een overval!" }, { Language.Turkey, "Bu bir soygun!" }
            }
        }
    };

    private string GetChatMessage(string phrase)
    {
        int langIndex = GameStateManager.CurrentLanguage;
        if (langIndex < 0) langIndex = 0;
        if (langIndex > 8) langIndex = 0;
        Language lang = (Language)langIndex;
        if (chatMessages.TryGetValue(phrase, out Dictionary<Language, string> dict) && dict.TryGetValue(lang, out string msg)) return msg;
        return "Message not found";
    }
    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            switch (request.code)
            {
                case "spawn": return SpawnCustomer(request);
                case "despawn": return DespawnCustomer(request);
                case "theft": return SpeechAll(request, CustomerSpeechType.THIS_IS_THEFT, null);
                case "robbery": return SpeechAll(request, null, "Robbery");
                case "soup": return SpeechAll(request, null, "Soup");
                case "breakfast": return SpeechAll(request, null, "Breakfast");
                case "boneless": return SpeechAll(request, null, "Pizza");
            }

            return EffectResponse.Failure(request.ID, "Unknown customer effect");
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"CustomerEffects error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    private EffectResponse SpawnCustomer(EffectRequest req)
    {
        if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        Transform door = DeliveryManager.Instance.m_DeliveryPosition;
        GameStateManager.NameOverride = req.viewer;
        Customer cust = CustomerGenerator.Instance.Spawn(door.position);
        cust.GoToStore(door.position);
        Il2CppSystem.Collections.Generic.List<Customer> active = CustomerManager.Instance.m_ActiveCustomers;
        active.Add(cust);
        CustomerManager.Instance.m_ActiveCustomers = active;
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse DespawnCustomer(EffectRequest req)
    {
        if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        Il2CppSystem.Collections.Generic.List<Customer> custList = CustomerManager.Instance.m_ActiveCustomers;
        bool despawned = false;
        try
        {
            foreach (Customer c in custList)
            {
                if (despawned) break;
                Checkout check = c.m_Checkout;
                bool shop = c.m_StartedShopping;
                if (shop && check == null)
                {
                    CustomerGenerator.Instance.DeSpawn(c);
                    despawned = true;
                    break;
                }

                if (shop && check != null)
                {
                    FieldInfo fi = typeof(Checkout).GetField("m_Customers", BindingFlags.Instance | BindingFlags.NonPublic);
                    Il2CppSystem.Collections.Generic.List<Customer> list = (Il2CppSystem.Collections.Generic.List<Customer>)fi.GetValue(check);
                    if (list != null && list.Count > 0 && list[0].GetInstanceID() != c.GetInstanceID())
                    {
                        c.OnDisable();
                        check.Unsubscribe(c);
                        CustomerGenerator.Instance.DeSpawn(c);
                        despawned = true;
                    }
                }
            }
        }
        catch { }

        return despawned ? EffectResponse.Success(req.ID) : EffectResponse.Retry(req.ID);
    }

    private EffectResponse SpeechAll(EffectRequest req, CustomerSpeechType? theftType, string phrase)
    {
        if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        Il2CppSystem.Collections.Generic.List<Customer> custList = CustomerManager.Instance.m_ActiveCustomers;
        bool found = false;
        foreach (Customer c in custList)
        {
            bool shop = c.m_StartedShopping;
            if (!shop) shop = c.m_InCheckout;
            if (shop)
            {
                found = true;
                break;
            }
        }

        if (!found) return EffectResponse.Retry(req.ID);
        foreach (Customer c in custList)
        {
            bool shop = c.m_StartedShopping;
            if (!shop) shop = c.m_InCheckout;
            if (!shop) continue;
            if (theftType.HasValue) WarningSystem.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT, c.transform, Array.Empty<string>());
            else
            {
                LocalizationEntry localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry();
                if (localizationEntry == null) continue;
                CustomerSpeech prefab = WarningSystem.Instance.m_CustomerSpeechPrefab;
                float time = WarningSystem.Instance.m_CustomerSpeechLifetime;
                CustomerSpeech speechObject = LeanPool.Spawn(prefab, c.transform);
                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, Array.Empty<string>());
                DOVirtual.DelayedCall(time, (Action)(() => LeanPool.Despawn(speechObject)), true);
                TMP_Text text = speechObject.m_SpeechText;
                if (phrase != null) text.text = GetChatMessage(phrase);
                speechObject.m_SpeechText = text;
            }
        }

        return EffectResponse.Success(req.ID);
    }
}