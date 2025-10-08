using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"open","close","lightson","lightsoff","upgrade","upgradeb","spawngarbage"})]
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
                    if (Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(request.ID);
                    Singleton<StoreStatus>.Instance.IsOpen = true; return EffectResponse.Success(request.ID);
                case "close":
                    if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(request.ID);
                    Singleton<StoreStatus>.Instance.IsOpen = false; return EffectResponse.Success(request.ID);
                case "lightson":
                    if (Singleton<StoreLightManager>.Instance.TurnOn) return EffectResponse.Retry(request.ID);
                    Singleton<StoreLightManager>.Instance.TurnOn = true; return EffectResponse.Success(request.ID);
                case "lightsoff":
                    if (!Singleton<StoreLightManager>.Instance.TurnOn) return EffectResponse.Retry(request.ID);
                    Singleton<StoreLightManager>.Instance.TurnOn = false; return EffectResponse.Success(request.ID);
                case "upgrade":
                    if (Singleton<SaveManager>.Instance.Progression.StoreUpgradeLevel >= 22) return EffectResponse.Failure(request.ID, "Store has reached max level.");
                    Singleton<SectionManager>.Instance.UpgradeStore();
                    Call(Singleton<SectionManager>.Instance,"PlayAnimations");
                    return EffectResponse.Success(request.ID);
                case "upgradeb":
                    if (!Singleton<SaveManager>.Instance.Storage.Purchased) return EffectResponse.Retry(request.ID);
                    if (Singleton<SaveManager>.Instance.Storage.StorageLevel >= 14) return EffectResponse.Failure(request.ID, "Storage has reached max level.");
                    Singleton<StorageSectionManager>.Instance.UpgradeStore();
                    Call(Singleton<StorageSectionManager>.Instance,"PlayAnimations");
                    return EffectResponse.Success(request.ID);
                case "spawngarbage":
                    var gm = GarbageManager.Instance; if (gm==null) return EffectResponse.Retry(request.ID);
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

    private static void Call(object obj,string name){obj.GetType().GetMethod(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public)?.Invoke(obj,null);}    
}

