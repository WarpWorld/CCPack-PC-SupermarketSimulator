using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"hirecashier","firecashier","hirerestocker","firerestocker","hirecustomerhelper","firecustomerhelper"})]
public class EmployeeEffects : Effect
{
    public EmployeeEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            switch(request.code)
            {
                case "hirecashier": return HireCashier(request);
                case "firecashier": return FireCashier(request);
                case "hirerestocker": return HireRestocker(request);
                case "firerestocker": return FireRestocker(request);
                case "hirecustomerhelper": return HireCustomerHelper(request);
                case "firecustomerhelper": return FireCustomerHelper(request);
            }
            return EffectResponse.Failure(request.ID, "Unknown employee effect");
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"Employee error: {e}"); return EffectResponse.Retry(request.ID);}        
    }

    private EffectResponse HireCashier(EffectRequest req)
    {
        var cashiers = (List<CashierSO>)GetField(Singleton<IDManager>.Instance,"m_Cashiers");
        if (Singleton<EmployeeManager>.Instance.CashiersData.Count >= 6) return EffectResponse.Failure(req.ID,"Can't hire any more cashiers.");
        if (!Singleton<SaveManager>.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        CashierSO target = null; foreach(var c in cashiers){ if(!Singleton<EmployeeManager>.Instance.CashiersData.Contains(c.ID)){ target = c; break; } }
        if (target==null || target.ID>6) return EffectResponse.Retry(req.ID);
        var cashierItems = UnityEngine.Object.FindObjectsOfType<CashierItem>();
        if (cashierItems.Length==0) Singleton<EmployeeManager>.Instance.HireCashier(target.ID,0,out _);
        else
        {
            foreach(var ci in cashierItems){ var hiredProp = ci.GetType().GetProperty("Hired",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public); if (hiredProp!=null && !(bool)hiredProp.GetValue(ci)){ ci.Hire(); break; } }
        }
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireCashier(EffectRequest req)
    {
        var em = Singleton<EmployeeManager>.Instance; if (em.CashiersData==null || em.CashiersData.Count<1) return EffectResponse.Failure(req.ID,"No cashiers to fire.");
        var items = UnityEngine.Object.FindObjectsOfType<CashierItem>();
        Singleton<EmployeeManager>.Instance.FireCashier(em.CashiersData[0]);
        foreach(var i in items) Call(i,"Start");
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse HireRestocker(EffectRequest req)
    {
        if (!Singleton<SaveManager>.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        var restockers = (List<RestockerSO>)GetField(Singleton<IDManager>.Instance,"m_Restockers");
        var owned = (List<int>)GetField(Singleton<EmployeeManager>.Instance,"m_RestockersData");
        if (owned.Count>=6) return EffectResponse.Failure(req.ID,"Max restockers hired.");
        var target = restockers.FirstOrDefault(r=>!owned.Contains(r.ID)); if (target==null) return EffectResponse.Retry(req.ID);
        var items = UnityEngine.Object.FindObjectsOfType<RestockerItem>();
        if (items.Length==0) Singleton<EmployeeManager>.Instance.HireRestocker(target.ID,0);
        else
        {
            foreach(var it in items){ var hiredProp = it.GetType().GetProperty("Hired",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public); if (hiredProp!=null && !(bool)hiredProp.GetValue(it)){ it.Hire(); break; } }
        }
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireRestocker(EffectRequest req)
    {
        if (!Singleton<SaveManager>.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        var owned = (List<int>)GetField(Singleton<EmployeeManager>.Instance,"m_RestockersData");
        if (owned==null || owned.Count==0) return EffectResponse.Failure(req.ID,"No Restockers to fire.");
        var items = UnityEngine.Object.FindObjectsOfType<RestockerItem>();
        Singleton<EmployeeManager>.Instance.FireRestocker(owned[0]);
        foreach(var it in items) Call(it,"Start");
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse HireCustomerHelper(EffectRequest req)
    {
        var helpers = (List<CustomerHelperSO>)GetField(Singleton<IDManager>.Instance,"m_CustomerHelpers");
        if (Singleton<EmployeeManager>.Instance.ActiveCustomerHelpers.Count >= 2) return EffectResponse.Failure(req.ID,"Can't hire any more customer helpers.");
        if (!Singleton<SaveManager>.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        CustomerHelperSO target = null; foreach(var h in helpers){ if(!Singleton<EmployeeManager>.Instance.CashiersData.Contains(h.ID)){ target = h; break; } }
        if (target==null || target.ID>2) return EffectResponse.Retry(req.ID);
        var items = UnityEngine.Object.FindObjectsOfType<CustomerHelperItem>();
        if (items.Length==0) Singleton<EmployeeManager>.Instance.HireCustomerHelper(target.ID,0,out _);
        else { foreach(var it in items){ var hiredProp = it.GetType().GetProperty("Hired",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public); if (hiredProp!=null && !(bool)hiredProp.GetValue(it)){ it.Hire(); break; } } }
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireCustomerHelper(EffectRequest req)
    {
        var em = Singleton<EmployeeManager>.Instance; if (em.ActiveCustomerHelpers==null || em.ActiveCustomerHelpers.Count<1) return EffectResponse.Failure(req.ID,"No customer helpers to fire.");
        var items = UnityEngine.Object.FindObjectsOfType<CustomerHelperItem>();
        Singleton<EmployeeManager>.Instance.FireCustomerHelper(Singleton<EmployeeManager>.Instance.CashiersData[0]);
        foreach(var it in items) Call(it,"Start");
        return EffectResponse.Success(req.ID);
    }

    private void Call(object obj,string name){obj.GetType().GetMethod(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public)?.Invoke(obj,null);}    
}