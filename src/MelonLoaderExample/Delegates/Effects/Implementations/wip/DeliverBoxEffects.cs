using ConnectorLib.JSON;
using UnityEngine;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"box_cereal","box_bread","box_milk","box_soda","box_eggs","box_salmon","box_mayo","box_whiskey","box_book","box_toilet","box_cat","box_lasag"})]
public class DeliverBoxEffects : Effect
{
    public DeliverBoxEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);

    private ProductSO GetProduct(string token)
    {
        foreach (int productID in Singleton<ProductLicenseManager>.Instance.AllPoducts)
        {
            ProductSO p = ProductSO(productID);
            if (p.ProductName.ToUpper().Contains(token.ToUpper())) return p;
        }
        return null;
    }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            string prod = request.code.Split('_')[1];
            var pos = (Transform)GetField(Singleton<DeliveryManager>.Instance,"m_DeliveryPosition");
            ProductSO product = GetProduct(prod);
            if (product==null) return EffectResponse.Failure(request.ID,"Product not found");
            Box box = Singleton<BoxGenerator>.Instance.SpawnBox(product, pos.position + Vector3.up * Singleton<DeliveryManager>.Instance.space * 1.0f, Quaternion.identity, pos);
            box.Setup(product.ID, true);
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"DeliverBox error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}

