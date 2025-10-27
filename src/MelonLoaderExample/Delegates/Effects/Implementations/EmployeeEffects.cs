using System;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Object = UnityEngine.Object;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("hirecashier", "firecashier", "hirerestocker", "firerestocker", "hirecustomerhelper", "firecustomerhelper")]
public class EmployeeEffects : Effect
{
    public EmployeeEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            switch (request.code)
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
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Employee error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    private EffectResponse HireCashier(EffectRequest req)
    {
        var cashiers = IDManager.Instance.m_Cashiers;
        if (EmployeeManager.Instance.CashiersData.Count >= 6) return EffectResponse.Failure(req.ID, "Can't hire any more cashiers.");
        if (!SaveManager.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        CashierSO target = null;
        foreach (CashierSO c in cashiers)
        {
            if (!EmployeeManager.Instance.CashiersData.Contains(c.ID))
            {
                target = c;
                break;
            }
        }

        if (target == null || target.ID > 6) return EffectResponse.Retry(req.ID);
        Il2CppArrayBase<CashierItem> cashierItems = Object.FindObjectsOfType<CashierItem>();
        if (cashierItems.Length == 0) EmployeeManager.Instance.HireCashier(target.ID, 0, out _);
        else
        {
            foreach (CashierItem ci in cashierItems)
                if (!ci.Hired) ci.Hire();
        }

        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireCashier(EffectRequest req)
    {
        EmployeeManager em = EmployeeManager.Instance;
        if (em.CashiersData == null || em.CashiersData.Count < 1) return EffectResponse.Failure(req.ID, "No cashiers to fire.");
        Il2CppArrayBase<CashierItem> items = Object.FindObjectsOfType<CashierItem>();
        EmployeeManager.Instance.FireCashier(em.CashiersData[0]);
        foreach (CashierItem it in items) it.Start();
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse HireRestocker(EffectRequest req)
    {
        if (!SaveManager.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        List<RestockerSO> restockers = IDManager.Instance.m_Restockers;
        List<int> owned = EmployeeManager.Instance.m_RestockersData;
        if (owned.Count >= 6) return EffectResponse.Failure(req.ID, "Max restockers hired.");
        RestockerSO target = restockers.FirstOrDefault(r => !owned.Contains(r.ID));
        if (target == null) return EffectResponse.Retry(req.ID);
        Il2CppArrayBase<RestockerItem> items = Object.FindObjectsOfType<RestockerItem>();
        if (items.Length == 0) EmployeeManager.Instance.HireRestocker(target.ID, 0);
        else
        {
            foreach (RestockerItem it in items)
                if (!it.Hired)
                    it.Hire();
        }

        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireRestocker(EffectRequest req)
    {
        if (!SaveManager.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        List<int> owned = EmployeeManager.Instance.m_RestockersData;
        if (owned == null || owned.Count == 0) return EffectResponse.Failure(req.ID, "No Restockers to fire.");
        Il2CppArrayBase<RestockerItem> items = Object.FindObjectsOfType<RestockerItem>();
        EmployeeManager.Instance.FireRestocker(owned[0]);
        foreach (RestockerItem it in items) it.Start();
        return EffectResponse.Success(req.ID);
    }

    private EffectResponse HireCustomerHelper(EffectRequest req)
    {
        List<CustomerHelperSO> helpers = IDManager.Instance.m_CustomerHelpers;
        if (EmployeeManager.Instance.ActiveCustomerHelpers.Count >= 2) return EffectResponse.Failure(req.ID, "Can't hire any more customer helpers.");
        if (!SaveManager.Instance.Storage.Purchased) return EffectResponse.Retry(req.ID);
        CustomerHelperSO target = null;
        foreach (CustomerHelperSO h in helpers)
        {
            if (!EmployeeManager.Instance.CashiersData.Contains(h.ID))
            {
                target = h;
                break;
            }
        }

        if (target == null || target.ID > 2) return EffectResponse.Retry(req.ID);
        Il2CppArrayBase<CustomerHelperItem> items = Object.FindObjectsOfType<CustomerHelperItem>();
        if (items.Length == 0) EmployeeManager.Instance.HireCustomerHelper(target.ID, 0, out _);
        else
        {
            foreach (CustomerHelperItem it in items)
                if (!it.Hired)
                    it.Hire();
        }

        return EffectResponse.Success(req.ID);
    }

    private EffectResponse FireCustomerHelper(EffectRequest req)
    {
        EmployeeManager em = EmployeeManager.Instance;
        if (em.ActiveCustomerHelpers == null || em.ActiveCustomerHelpers.Count < 1) return EffectResponse.Failure(req.ID, "No customer helpers to fire.");
        Il2CppArrayBase<CustomerHelperItem> items = Object.FindObjectsOfType<CustomerHelperItem>();
        EmployeeManager.Instance.FireCustomerHelper(EmployeeManager.Instance.CashiersData[0]);
        foreach (CustomerHelperItem it in items) it.Start();
        return EffectResponse.Success(req.ID);
    }
}