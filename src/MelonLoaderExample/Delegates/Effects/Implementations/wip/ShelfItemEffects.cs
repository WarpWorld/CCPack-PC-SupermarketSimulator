using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using Display = UnityEngine.Display;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"removeitem","additem"})]
public class ShelfItemEffects : Effect
{
    public ShelfItemEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);
    private static void SetField(object o,string n,object v)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(o,v);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var displays = (List<Display>)GetField(Singleton<DisplayManager>.Instance,"m_Displays");
            if (displays==null || displays.Count==0) return EffectResponse.Retry(request.ID);
            List<DisplaySlot> targets = new();
            foreach (var display in displays)
            {
                var slots = (DisplaySlot[])GetField(display,"m_DisplaySlots");
                foreach (var slot in slots)
                {
                    var data = (ItemQuantity)GetField(slot,"m_ProductCountData");
                    if (request.code=="removeitem")
                    {
                        if (data!=null && data.HasProduct && data.FirstItemCount>0) targets.Add(slot);
                    }
                    else // additem
                    {
                        if (data!=null && data.HasLabel && !slot.Full) targets.Add(slot);
                    }
                }
            }
            if (targets.Count==0) return EffectResponse.Retry(request.ID);
            var rnd = CrowdDelegates.rnd;
            var target = targets[rnd.Next(targets.Count)];
            if (request.code=="removeitem")
            {
                try{ var prod = target.TakeProductFromDisplay(); Lean.Pool.LeanPool.Despawn(prod,0f);}catch{}
            }
            else
            {
                var data = (ItemQuantity)GetField(target,"m_ProductCountData");
                var productSO = ProductSO(data.FirstItemID);
                var product = Lean.Pool.LeanPool.Spawn<Product>(productSO.ProductPrefab, target.transform, false);
                target.AddProduct(data.FirstItemID, product);
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"ShelfItem error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}

