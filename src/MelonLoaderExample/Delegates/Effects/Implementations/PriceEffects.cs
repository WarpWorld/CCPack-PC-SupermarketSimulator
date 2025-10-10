using System;
using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using Random = UnityEngine.Random;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("pricesup", "pricesdown", "priceup", "pricedown")]
public class PriceEffects : Effect
{
    public PriceEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            List<Pricing> prices = PriceManager.Instance.m_PricesSetByPlayer;
            if (prices == null || prices.Count == 0) return EffectResponse.Retry(request.ID);
            switch (request.code)
            {
                case "pricesup":
                    foreach (Pricing p in prices)
                    {
                        p.Price *= 1.1f;
                        PriceManager.Instance.PriceSet(p);
                    }

                    break;
                case "pricesdown":
                    foreach (Pricing p in prices)
                    {
                        p.Price *= 0.8f;
                        if (p.Price < 0.25f) p.Price = 0.25f;
                        PriceManager.Instance.PriceSet(p);
                    }

                    break;
                case "priceup":
                {
                    int r = Random.Range(0, prices.Count);
                    Pricing p = prices[r];
                    p.Price *= 1.1f;
                    PriceManager.Instance.PriceSet(p);
                    break;
                }
                case "pricedown":
                {
                    List<Pricing> candidates = new List<Pricing>();
                    foreach (Pricing p in prices)
                        if (p.Price > 0.25f)
                            candidates.Add(p);
                    if (candidates.Count == 0) return EffectResponse.Retry(request.ID);
                    int r = Random.Range(0, candidates.Count);
                    Pricing p2 = candidates[r];
                    p2.Price *= 0.8f;
                    if (p2.Price < 0.25f) p2.Price = 0.25f;
                    PriceManager.Instance.PriceSet(p2);
                    break;
                }
            }

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"PriceEffects error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }
}