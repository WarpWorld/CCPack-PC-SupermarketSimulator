using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"forcepayment_cash","forcepayment_card"})]
public class ForcePaymentTypeEffects : Effect
{
    public ForcePaymentTypeEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(request.ID);
            var player = Singleton<PlayerInteraction>.Instance; if (!player.InInteraction) return EffectResponse.Retry(request.ID);
            if (GameStateManager.currentHeldItem != "CHECKOUT") return EffectResponse.Retry(request.ID);

            if (request.code == "forcepayment_cash") { GameStateManager.ForceUseCash = true; GameStateManager.ForceUseCredit = false; }
            else { GameStateManager.ForceUseCredit = true; GameStateManager.ForceUseCash = false; }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"ForcePaymentType error: {e}"); return EffectResponse.Retry(request.ID);}        
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        GameStateManager.ForceUseCash = false; GameStateManager.ForceUseCredit = false; return EffectResponse.Finished(request.ID);
    }
}

