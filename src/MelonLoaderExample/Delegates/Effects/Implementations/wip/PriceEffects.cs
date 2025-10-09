using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using Random = Il2CppSystem.Random;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"pricesup","pricesdown","priceup","pricedown"})]
public class PriceEffects : Effect
{
    public PriceEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }
    private static Random rnd = new();
    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var prices = (List<Pricing>)GetField(Singleton<PriceManager>.Instance,"m_PricesSetByPlayer");
            if (prices==null || prices.Count==0) return EffectResponse.Retry(request.ID);
            switch(request.code)
            {
                case "pricesup":
                    foreach(var p in prices){ p.Price *= 1.1f; Singleton<PriceManager>.Instance.PriceSet(p);} break;
                case "pricesdown":
                    foreach(var p in prices){ p.Price *= 0.8f; if (p.Price < 0.25f) p.Price = 0.25f; Singleton<PriceManager>.Instance.PriceSet(p);} break;
                case "priceup":
                    {
                        int r = rnd.Next(prices.Count); var p = prices[r]; p.Price *= 1.1f; Singleton<PriceManager>.Instance.PriceSet(p); break;
                    }
                case "pricedown":
                    {
                        var candidates = new List<Pricing>(); foreach(var p in prices) if (p.Price > 0.25f) candidates.Add(p); if (candidates.Count==0) return EffectResponse.Retry(request.ID); int r = rnd.Next(candidates.Count); var p2 = candidates[r]; p2.Price *= 0.8f; if (p2.Price < 0.25f) p2.Price = 0.25f; Singleton<PriceManager>.Instance.PriceSet(p2); break;
                    }
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"PriceEffects error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}