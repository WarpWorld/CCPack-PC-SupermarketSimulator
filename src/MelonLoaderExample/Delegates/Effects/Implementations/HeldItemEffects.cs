using System;
using ConnectorLib.JSON;
using Il2Cpp;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("drop", "throw")]
public class HeldItemEffects : Effect
{
    public HeldItemEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            PlayerObjectHolder holder = UnityEngine.Object.FindObjectOfType<PlayerObjectHolder>();
            PlayerInteraction player = UnityEngine.Object.FindObjectOfType<PlayerInteraction>();
            GameObject obj = holder.m_CurrentObject;
            if (obj == null) return EffectResponse.Retry(request.ID);
            bool inInteraction = player.m_InInteraction;
            if (!inInteraction) return EffectResponse.Retry(request.ID);
            if (GameStateManager.currentHeldItem == null || !GameStateManager.currentHeldItem.Contains("BOX")) return EffectResponse.Retry(request.ID);
            if (request.code == "throw")
            {
                holder.ThrowObject();
            }
            else
            {
                holder.DropObject();
                player.m_InInteraction = false;
                //player.m_PlacingMode = false; //todo: field doesn't exist in il2cpp??
            }

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"HeldItem error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }
}