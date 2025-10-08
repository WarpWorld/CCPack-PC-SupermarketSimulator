using ConnectorLib.JSON;
using UnityEngine;
using Il2Cpp;
using Il2CppPG;
using Il2CppTMPro;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"spawn","despawn","theft","robbery","soup","breakfast","boneless"})]
public class CustomerEffects : Effect
{
    public CustomerEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private enum Language { English=0,French=1,German=2,Italian=3,Spanish=4,Portugal=5,Brazil=6,Netherlands=7,Turkey=8 }
    private static readonly Dictionary<string, Dictionary<Language,string>> chatMessages = new(){
        {"Pizza", new(){ {Language.English,"Lemme get that pizza BONELESS!"},{Language.French,"Laisse-moi avoir cette pizza SANS OS !"},{Language.German,"Lass mir diese Pizza OHNE KNOCHEN!"},{Language.Italian,"Dammi quella pizza SENZA OSSA!"},{Language.Spanish,"¡Dame esa pizza SIN HUESOS!"},{Language.Portugal,"Deixa-me ter essa pizza SEM OSSOS!"},{Language.Brazil,"Deixa eu pegar essa pizza SEM OSSO!"},{Language.Netherlands,"Laat me die pizza ZONDER BOTTEN hebben!"},{Language.Turkey,"Bana kemiksiz o pizzayı ver!"} }},
        {"Soup", new(){ {Language.English,"I'm at soup!"},{Language.French,"Je suis à la soupe !"},{Language.German,"Ich bin bei der Suppe!"},{Language.Italian,"Sono alla zuppa!"},{Language.Spanish,"¡Estoy en la sopa!"},{Language.Portugal,"Estou na sopa!"},{Language.Brazil,"Estou na sopa!"},{Language.Netherlands,"Ik ben bij de soep!"},{Language.Turkey,"Çorbadayım!"} }},
        {"Breakfast", new(){ {Language.English,"It's breakfast time."},{Language.French,"C'est l'heure du petit-déjeuner."},{Language.German,"Es ist Frühstückszeit."},{Language.Italian,"È ora di colazione."},{Language.Spanish,"Es hora del desayuno."},{Language.Portugal,"É hora do café da manhã."},{Language.Brazil,"É hora do café da manhã."},{Language.Netherlands,"Het is ontbijttijd."},{Language.Turkey,"Kahvaltı zamanı."} }},
        {"Robbery", new(){ {Language.English,"This is a robbery!"},{Language.French,"C'est un vol !"},{Language.German,"Das ist ein Überfall!"},{Language.Italian,"Questa è una rapina!"},{Language.Spanish,"¡Esto es un robo!"},{Language.Portugal,"Isto é um assalto!"},{Language.Brazil,"Isto é um assalto!"},{Language.Netherlands,"Dit is een overval!"},{Language.Turkey,"Bu bir soygun!"} }}
    };

    private string GetChatMessage(string phrase)
    {
        int langIndex = GameStateManager.CurrentLanguage; if (langIndex<0) langIndex=0; if (langIndex>8) langIndex=0;
        var lang = (Language)langIndex;
        if (chatMessages.TryGetValue(phrase, out var dict) && dict.TryGetValue(lang,out var msg)) return msg;
        return "Message not found";
    }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);
    private static void SetField(object o,string n,object v)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(o,v);
    private static void Call(object o,string n)=>o.GetType().GetMethod(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public)?.Invoke(o,null);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            switch(request.code)
            {
                case "spawn": return SpawnCustomer(request);
                case "despawn": return DespawnCustomer(request);
                case "theft": return SpeechAll(request,CustomerSpeechType.THIS_IS_THEFT,null);
                case "robbery": return SpeechAll(request,null,"Robbery");
                case "soup": return SpeechAll(request,null,"Soup");
                case "breakfast": return SpeechAll(request,null,"Breakfast");
                case "boneless": return SpeechAll(request,null,"Pizza");
            }
            return EffectResponse.Failure(request.ID,"Unknown customer effect");
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"CustomerEffects error: {e}"); return EffectResponse.Retry(request.ID);}        
    }

    private EffectResponse SpawnCustomer(EffectRequest req)
    {
        if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        var door = (Transform)GetField(Singleton<DeliveryManager>.Instance,"m_DeliveryPosition");
        GameStateManager.NameOverride = req.viewer;
        var cust = Singleton<CustomerGenerator>.Instance.Spawn(door.position);
        cust.GoToStore(door.position);
        var active = (List<Customer>)GetField(Singleton<CustomerManager>.Instance,"m_ActiveCustomers");
        active.Add(cust);
        SetField(Singleton<CustomerManager>.Instance,"m_ActiveCustomers",active);
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse DespawnCustomer(EffectRequest req)
    {
        if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        var custList = (List<Customer>)GetField(Singleton<CustomerManager>.Instance,"m_ActiveCustomers");
        bool despawned=false;
        try
        {
            foreach(var c in custList)
            {
                if (despawned) break;
                var check = (Checkout)GetField(c,"m_Checkout");
                bool shop = (bool)GetField(c,"m_StartedShopping");
                if (shop && check==null){ Singleton<CustomerGenerator>.Instance.DeSpawn(c); despawned=true; break; }
                if (shop && check!=null)
                {
                    var fi = typeof(Checkout).GetField("m_Customers",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
                    var list = (List<Customer>)fi.GetValue(check);
                    if (list!=null && list.Count>0 && list[0].GetInstanceID()!=c.GetInstanceID())
                    { Call(c,"OnDisable"); check.Unsubscribe(c); Singleton<CustomerGenerator>.Instance.DeSpawn(c); despawned=true; }
                }
            }
        }catch{}
        return despawned? EffectResponse.Success(req.ID): EffectResponse.Retry(req.ID);
    }

    private EffectResponse SpeechAll(EffectRequest req, CustomerSpeechType? theftType, string phrase)
    {
        if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(req.ID);
        var custList = (List<Customer>)GetField(Singleton<CustomerManager>.Instance,"m_ActiveCustomers");
        bool found=false;
        foreach(var c in custList){ bool shop=(bool)GetField(c,"m_StartedShopping"); if(!shop) shop=(bool)GetField(c,"m_InCheckout"); if(shop){found=true; break;} }
        if(!found) return EffectResponse.Retry(req.ID);
        foreach(var c in custList)
        {
            bool shop=(bool)GetField(c,"m_StartedShopping"); if(!shop) shop=(bool)GetField(c,"m_InCheckout"); if(!shop) continue;
            if (theftType.HasValue) Singleton<WarningSystem>.Instance.SpawnCustomerSpeech(CustomerSpeechType.THIS_IS_THEFT,c.transform,Array.Empty<string>());
            else
            {
                var localizationEntry = CustomerSpeechType.THIS_IS_THEFT.LocalizationEntry(); if (localizationEntry==null) continue;
                var prefab = (CustomerSpeech)GetField(Singleton<WarningSystem>.Instance,"m_CustomerSpeechPrefab");
                float time = (float)GetField(Singleton<WarningSystem>.Instance,"m_CustomerSpeechLifetime");
                var speechObject = Lean.Pool.LeanPool.Spawn<CustomerSpeech>(prefab, c.transform, false);
                speechObject.Setup(localizationEntry.TableCollection, localizationEntry.TableEntry, Array.Empty<string>());
                DOVirtual.DelayedCall(time, ()=>{ Lean.Pool.LeanPool.Despawn(speechObject,0f);}, true);
                var text = (TMP_Text)GetField(speechObject,"m_SpeechText");
                if (phrase!=null) text.text = GetChatMessage(phrase);
                SetField(speechObject,"m_SpeechText", text);
            }
        }
        return EffectResponse.Success(req.ID);
    }
}

