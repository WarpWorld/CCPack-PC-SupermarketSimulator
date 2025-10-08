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

/*
[Effect(new[]{"teleport_outsidestore","teleport_acrossstreet","teleport_faraway","teleport_computer"})]
public class TeleportEffects : Effect
{
    public TeleportEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var player = Singleton<PlayerInteraction>.Instance;
            if (player!=null && player.InInteraction && (GameStateManager.currentHeldItem=="COMPUTER"|| GameStateManager.currentHeldItem=="CHECKOUT"))
                return EffectResponse.Retry(request.ID);

            var pcTransform = Singleton<PlayerController>.Instance.transform;
            string location = request.code.Split('_')[1];
            Vector3 teleportPosition = pcTransform.position;
            switch(location)
            {
                case "outsidestore":
                    teleportPosition = ( (Transform)GetField(Singleton<DeliveryManager>.Instance,"m_DeliveryPosition")).position; break;
                case "acrossstreet": teleportPosition = new Vector3(32.07f,-0.07f,4.14f); break;
                case "computer":
                    var tData = Singleton<SaveManager>.Instance.Progression.ComputerTransform;
                    var compPos = tData.Position; var compRot = tData.Rotation; var fwd = compRot * Vector3.forward; teleportPosition = compPos - fwd * 1.0f; break;
                case "faraway":
                    Vector3[] positions = { new(-64.75f,-0.04f,46.34f), new(90.38f,-0.06f,-52.40f), new(-65.27f,-0.06f,-24.99f)}; var r=new System.Random(); teleportPosition = positions[r.Next(0,positions.Length)]; break;
            }
            pcTransform.position = teleportPosition;
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"Teleport error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}
*/
