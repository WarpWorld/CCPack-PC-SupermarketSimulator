using ConnectorLib.JSON;
using Il2CppAssets.Scripts.Actors.Player;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("healPlayer")]
public class HealPlayer : Effect
{
    public HealPlayer(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {

            //CrowdControlMod.Instance.Logger.Msg($"HP: {MyPlayer.Instance.inventory.playerHealth.hp}");
            //CrowdControlMod.Instance.Logger.Msg($"MaxHP: {MyPlayer.Instance.inventory.playerHealth.maxHp}");

            if (MyPlayer.Instance && MyPlayer.Instance.inventory.playerHealth.hp != MyPlayer.Instance.inventory.playerHealth.maxHp)
            {
                MyPlayer.Instance.inventory.playerHealth.Heal(MyPlayer.Instance.inventory.playerHealth.maxHp);
                return EffectResponse.Success(request.ID);
            }

            return EffectResponse.Failure(request.ID, "Cannot heal player at this time.");
        }
        catch (Exception ex)
        {
            CrowdControlMod.Instance.Logger.Error($"Error healing player: {ex.Message}");
            return EffectResponse.Failure(request.ID, $"Error healing player: {ex.Message}");
        }
    }
}
