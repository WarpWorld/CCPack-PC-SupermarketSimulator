using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;
using UnityEngine;
using PlayerController = Il2CppPG.PlayerController;

namespace CrowdControl.Delegates.Effects.Implementations;

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
                    teleportPosition = ((Transform)GetField(Singleton<DeliveryManager>.Instance,"m_DeliveryPosition")).position; break;
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

