using System;
using ConnectorLib.JSON;
using Il2Cpp;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("box_cereal", "box_bread", "box_milk", "box_soda", "box_eggs", "box_salmon", "box_mayo", "box_whiskey", "box_book", "box_toilet", "box_cat", "box_lasag")]
public class DeliverBoxEffects : BoxEffectBase
{
    public DeliverBoxEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            string prod = request.code.Split('_')[1];
            Transform pos = DeliveryManager.Instance.m_DeliveryPosition;
            ProductSO product = GetProduct(prod);
            if (product==null) return EffectResponse.Failure(request.ID,"Product not found");
            Box box = BoxGenerator.Instance.SpawnBox(product, pos.position + Vector3.up * DeliveryManager.Instance.space * 1.0f, Quaternion.identity, pos);
            box.Setup(product.ID, true);
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"DeliverBox error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}