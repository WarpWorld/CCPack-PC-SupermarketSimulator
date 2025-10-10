using System;
using ConnectorLib.JSON;
using Il2Cpp;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("playerbox_cereal", "playerbox_bread", "playerbox_milk", "playerbox_soda", "playerbox_eggs", "playerbox_salmon", "playerbox_mayo", "playerbox_whiskey", "playerbox_book", "playerbox_toilet", "playerbox_cat", "playerbox_lasag", "playeremptybox_eggs", "playeremptybox_cereal", "playeremptybox_toilet")]
public class PlayerBoxEffects : BoxEffectBase
{
    public PlayerBoxEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }
    
    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            string prod = request.code.Split('_')[1];
            PlayerController pc = UnityEngine.Object.FindObjectOfType<PlayerController>();
            Transform playerPos = pc.gameObject.transform;
            ProductSO product = GetProduct(prod);
            if (product==null) return EffectResponse.Failure(request.ID,"Product not found");
            bool empty = request.code.StartsWith("playeremptybox_");
            Box box = BoxGenerator.Instance.SpawnBox(product, playerPos.position + Vector3.up * DeliveryManager.Instance.space * 2.0f, Quaternion.identity, null);
            if (!empty) box.Setup(product.ID, true);
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"PlayerBox error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}