using System;
using ConnectorLib.JSON;
using Il2Cpp;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("forceexactchange", "forcerequirechange", "forcelargebills", "allowmischarges", "forcemath")]
public class PaymentModifiersEffects : Effect
{
    public PaymentModifiersEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(request.ID);
            PlayerInteraction player = UnityEngine.Object.FindObjectOfType<PlayerInteraction>();
            if (!player.InInteraction) return EffectResponse.Retry(request.ID);
            if (GameStateManager.currentHeldItem != "CHECKOUT") return EffectResponse.Retry(request.ID);

            switch(request.code)
            {
                case "forceexactchange": GameStateManager.ForceExactChange = true; break;
                case "forcerequirechange": GameStateManager.ForceRequireChange = true; break;
                case "forcelargebills": GameStateManager.ForceLargeBills = true; break;
                case "allowmischarges": GameStateManager.AllowMischarge = true; break;
                case "forcemath": GameStateManager.ForceMath = true; break;
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"PaymentModifiers error: {e}"); return EffectResponse.Retry(request.ID);}        
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        GameStateManager.ForceExactChange = false;
        GameStateManager.ForceRequireChange = false;
        GameStateManager.ForceLargeBills = false;
        GameStateManager.AllowMischarge = false;
        GameStateManager.ForceMath = false;
        return EffectResponse.Finished(request.ID);
    }
}

