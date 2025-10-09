using System;
using System.Reflection;
using ConnectorLib.JSON;
using Il2Cpp;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("open", "close", "lightson", "lightsoff", "upgrade", "upgradeb", "spawngarbage")]
public class StoreEffects : Effect
{
    public StoreEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            switch (request.code)
            {
                case "open":
                    if (StoreStatus.Instance.IsOpen) return EffectResponse.Retry(request.ID);
                    StoreStatus.Instance.IsOpen = true; return EffectResponse.Success(request.ID);
                case "close":
                    if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(request.ID);
                    StoreStatus.Instance.IsOpen = false; return EffectResponse.Success(request.ID);
                case "lightson":
                    if (StoreLightManager.Instance.TurnOn) return EffectResponse.Retry(request.ID);
                    StoreLightManager.Instance.TurnOn = true; return EffectResponse.Success(request.ID);
                case "lightsoff":
                    if (!StoreLightManager.Instance.TurnOn) return EffectResponse.Retry(request.ID);
                    StoreLightManager.Instance.TurnOn = false; return EffectResponse.Success(request.ID);
                case "upgrade":
                    if (SaveManager.Instance.Progression.StoreUpgradeLevel >= 22) return EffectResponse.Failure(request.ID, "Store has reached max level.");
                    SectionManager.Instance.UpgradeStore();
                    Call(SectionManager.Instance,"PlayAnimations");
                    return EffectResponse.Success(request.ID);
                case "upgradeb":
                    if (!SaveManager.Instance.Storage.Purchased) return EffectResponse.Retry(request.ID);
                    if (SaveManager.Instance.Storage.StorageLevel >= 14) return EffectResponse.Failure(request.ID, "Storage has reached max level.");
                    StorageSectionManager.Instance.UpgradeStore();
                    Call(StorageSectionManager.Instance,"PlayAnimations");
                    return EffectResponse.Success(request.ID);
                case "spawngarbage":
                    GarbageManager gm = GarbageManager.Instance; if (gm==null) return EffectResponse.Retry(request.ID);
                    gm.SpawnGarbage(); return EffectResponse.Success(request.ID);
            }
            return EffectResponse.Failure(request.ID, "Unknown store effect");
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"StoreEffects error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    private static void Call(object obj,string name){obj.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public)?.Invoke(obj,null);}    
}

