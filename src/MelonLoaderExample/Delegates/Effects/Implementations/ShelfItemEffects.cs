using System;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppLean.Pool;
using Il2CppSystem.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("removeitem", "additem")]
public class ShelfItemEffects : Effect
{
    public ShelfItemEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            List<Display> displays = DisplayManager.Instance.m_Displays;
            if (displays == null || displays.Count == 0) return EffectResponse.Retry(request.ID);
            List<DisplaySlot> targets = new();
            foreach (Display display in displays)
            {
                DisplaySlot[] slots = display.m_DisplaySlots;
                foreach (DisplaySlot slot in slots)
                {
                    ItemQuantity data = slot.m_ProductCountData;
                    if (request.code == "removeitem")
                    {
                        if (data != null && data.HasProduct && data.FirstItemCount > 0) targets.Add(slot);
                    }
                    else // additem
                    {
                        if (data != null && data.HasLabel && !slot.Full) targets.Add(slot);
                    }
                }
            }

            if (targets.Count == 0) return EffectResponse.Retry(request.ID);
            
            DisplaySlot target = targets[Random.Range(0, targets.Count)];
            if (request.code == "removeitem")
            {
                Product prod = target.TakeProductFromDisplay();
                LeanPool.Despawn(prod);
            }
            else
            {
                ItemQuantity data = target.m_ProductCountData;
                ProductSO productSO = IDManager.Instance.ProductSO(data.FirstItemID);
                Product product = LeanPool.Spawn(productSO.ProductPrefab, target.transform);
                target.AddProduct(data.FirstItemID, product);
            }

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"ShelfItem error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }
}