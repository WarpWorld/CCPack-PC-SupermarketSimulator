using ConnectorLib.JSON;
using Il2CppAssets.Scripts.Actors.Player;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("goldUp", "goldDown")]
public class AddRemoveGold : Effect
{
    public AddRemoveGold(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        if (MyPlayer.Instance is null) return EffectResponse.Failure(request.ID);

        float quantity = request.quantity ?? 0f;
        if (quantity == 0) return EffectResponse.Failure(request.ID);

        // Convert to negative if code is goldDown
        if (string.Equals("goldDown", request.code, StringComparison.OrdinalIgnoreCase))
            quantity *= -1;


        MyPlayer.Instance.inventory.ChangeGold(UnityEngine.Mathf.RoundToInt(quantity));
        return EffectResponse.Success(request.ID);
    }
}
