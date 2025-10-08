using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using UnityEngine;
using PlayerController = Il2CppPG.PlayerController;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"playerbox_cereal","playerbox_bread","playerbox_milk","playerbox_soda","playerbox_eggs","playerbox_salmon","playerbox_mayo","playerbox_whiskey","playerbox_book","playerbox_toilet","playerbox_cat","playerbox_lasag","playeremptybox_eggs","playeremptybox_cereal","playeremptybox_toilet"})]
public class PlayerBoxEffects : Effect
{
    public PlayerBoxEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);
    private ProductSO GetProduct(string token){foreach (int productID in Singleton<ProductLicenseManager>.Instance.AllPoducts){ProductSO p = ProductSO(productID); if (p.ProductName.ToUpper().Contains(token.ToUpper())) return p;} return null;}

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            string prod = request.code.Split('_')[1];
            var playerPos = Singleton<PlayerController>.Instance.transform;
            ProductSO product = GetProduct(prod);
            if (product==null) return EffectResponse.Failure(request.ID,"Product not found");
            bool empty = request.code.StartsWith("playeremptybox_");
            Box box = Singleton<BoxGenerator>.Instance.SpawnBox(product, playerPos.position + Vector3.up * Singleton<DeliveryManager>.Instance.space * 2.0f, Quaternion.identity, null);
            if (!empty) box.Setup(product.ID, true);
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"PlayerBox error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}

