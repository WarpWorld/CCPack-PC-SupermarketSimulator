using ConnectorLib.JSON;
using Il2CppPG;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("event-hype-train")]
public class HypeTrainEffect : Effect
{
    private static bool _loaded;
    private static AssetBundle _bundle;
    private static GameObject _prefab;

    public HypeTrainEffect(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private void Load()
    {
        if (_loaded) return;
        var path = System.IO.Path.Combine(Paths.PluginPath, "CrowdControl", "warpworld.hypetrain");
        _bundle = AssetBundle.LoadFromFile(path);
        if (_bundle!=null) _prefab = _bundle.LoadAsset<GameObject>("HypeTrain");
        _loaded = true;
    }

    private static UnityEngine.Color NameToColor(string name)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(name));
        return new Color(bytes[0]/255f, bytes[1]/255f, bytes[2]/255f);
    }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            Load();
            if (_prefab==null) return EffectResponse.Failure(request.ID,"prefab missing");
            var cam = Camera.main?.transform ?? UnityEngine.Object.FindObjectOfType<Camera>()?.transform; if (!cam) return EffectResponse.Failure(request.ID,"no camera");
            var forward = cam.forward;
            var pos = cam.position + forward * 0.5f + Vector3.up * 1.0f;
            var hype = UnityEngine.Object.Instantiate(_prefab, pos, Quaternion.identity).GetComponent<HypeTrain>();
            if (hype==null) return EffectResponse.Failure(request.ID,"component missing");
            List<HypeTrainBoxData> list = new();
            if (request.sourceDetails.top_contributions.Length==0) return EffectResponse.Success(request.ID); // nothing to show
            foreach(var c in request.sourceDetails.top_contributions)
            {
                list.Add(new HypeTrainBoxData{ name = c.user_name, box_color = NameToColor(c.user_name), bit_amount = c.type=="bits"? c.total:0 });
            }
            if (request.sourceDetails.last_contribution!=null)
            {
                list.Add(new HypeTrainBoxData{ name = request.sourceDetails.last_contribution.user_name, box_color = NameToColor(request.sourceDetails.last_contribution.user_name), bit_amount = request.sourceDetails.last_contribution.type=="bits"? request.sourceDetails.last_contribution.total:0 });
            }
            var playerT = Singleton<PlayerController>.Instance.transform;
            Vector3 startPos = playerT.position + cam.TransformDirection(new Vector3(-14.5f,0.2f,6.0f)); startPos.y = playerT.position.y;
            Vector3 stopPos = playerT.position + cam.TransformDirection(new Vector3(14.5f,0.2f,6.0f)); stopPos.y = playerT.position.y;
            hype.StartHypeTrain(startPos, stopPos, list.ToArray(), playerT, new HypeTrainOptions{ train_layer = LayerMask.NameToLayer("FloorLayer"), max_bits_per_car = 100 });
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"HypeTrain error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}

/* ShelfItemEffects moved to separate file ShelfItemEffects.cs
using ConnectorLib.JSON;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                var productSO = Singleton<IDManager>.Instance.ProductSO(data.FirstItemID);
                var product = Lean.Pool.LeanPool.Spawn<Product>(productSO.ProductPrefab, target.transform, false);
                target.AddProduct(data.FirstItemID, product);
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"ShelfItem error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}
*/
