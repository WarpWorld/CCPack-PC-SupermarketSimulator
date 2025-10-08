using ConnectorLib.JSON;
using Il2CppAssets.Scripts.Actors.Player;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("addXP")]
public class AddXP : Effect
{
    public AddXP(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        if (MyPlayer.Instance is null)
            return EffectResponse.Failure(request.ID);

        float quantity = request.quantity ?? 0f;
        if (quantity == 0f)
            return EffectResponse.Failure(request.ID);


        int addXP = UnityEngine.Mathf.RoundToInt(quantity);

        MyPlayer.Instance.inventory.AddXp(addXP);

        return EffectResponse.Success(request.ID);
    }
}