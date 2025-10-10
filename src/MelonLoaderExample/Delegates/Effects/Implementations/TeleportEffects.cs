using System;
using ConnectorLib.JSON;
using Il2Cpp;
using UnityEngine;
using Random = System.Random;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("teleport_outsidestore", "teleport_acrossstreet", "teleport_faraway", "teleport_computer")]
public class TeleportEffects : Effect
{
    public TeleportEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            PlayerInteraction player = UnityEngine.Object.FindObjectOfType<PlayerInteraction>();
            if (player != null && player.InInteraction && (GameStateManager.currentHeldItem == "COMPUTER" || GameStateManager.currentHeldItem == "CHECKOUT"))
                return EffectResponse.Retry(request.ID);

            PlayerController pc = UnityEngine.Object.FindObjectOfType<PlayerController>();
            Transform pcTransform = pc.gameObject.transform;
            string location = request.code.Split('_')[1];
            Vector3 teleportPosition = pcTransform.position;
            switch (location)
            {
                case "outsidestore":
                    teleportPosition = DeliveryManager.Instance.m_DeliveryPosition.position;
                    break;
                case "acrossstreet": teleportPosition = new(32.07f, -0.07f, 4.14f); break;
                case "computer":
                    TransformData tData = SaveManager.Instance.Progression.ComputerTransform;
                    Vector3 compPos = tData.Position;
                    Quaternion compRot = tData.Rotation;
                    Vector3 fwd = compRot * Vector3.forward;
                    teleportPosition = compPos - fwd * 1.0f;
                    break;
                case "faraway":
                    Vector3[] positions = { new(-64.75f, -0.04f, 46.34f), new(90.38f, -0.06f, -52.40f), new(-65.27f, -0.06f, -24.99f) };
                    Random r = new Random();
                    teleportPosition = positions[r.Next(0, positions.Length)];
                    break;
            }

            pcTransform.position = teleportPosition;
            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Teleport error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }
}