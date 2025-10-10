using System;
using ConnectorLib.JSON;
using Il2Cpp;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("clearunusedboxes")]
public class ClearUnusedBoxesEffect : Effect
{
    public ClearUnusedBoxesEffect(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
            if (boxes.Length < 1) return EffectResponse.Failure(request.ID, "No boxes to despawn.");
            foreach (GameObject b in boxes)
            {
                Box comp = b.GetComponent<Box>();
                if (comp) b.SetActive(false);
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"ClearBoxes error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}