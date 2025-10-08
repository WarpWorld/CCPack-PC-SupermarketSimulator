using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"drop","throw"})]
public class HeldItemEffects : Effect
{
    public HeldItemEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object o,string n)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(o);
    private static void SetField(object o,string n,object v)=>o.GetType().GetField(n,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(o,v);

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var holder = Singleton<PlayerObjectHolder>.Instance; var player = Singleton<PlayerInteraction>.Instance;
            var obj = (GameObject)GetField(holder,"m_CurrentObject"); if (obj==null) return EffectResponse.Retry(request.ID);
            var inInteraction = (bool)GetField(player,"m_InInteraction"); if (!inInteraction) return EffectResponse.Retry(request.ID);
            if (GameStateManager.currentHeldItem==null || !GameStateManager.currentHeldItem.Contains("BOX")) return EffectResponse.Retry(request.ID);
            if (request.code=="throw")
            {
                Singleton<PlayerObjectHolder>.Instance.ThrowObject();
            }
            else
            {
                Singleton<PlayerObjectHolder>.Instance.DropObject();
                SetField(player,"m_InInteraction", false);
                SetField(player,"m_PlacingMode", false);
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"HeldItem error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}

