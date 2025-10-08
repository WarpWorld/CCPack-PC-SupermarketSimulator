using ConnectorLib.JSON;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[] {"lowfov","highfov"}, 30, new[]{"lowfov","highfov"})]
public class FOVEffects : Effect
{
    private float _originalFov = 60f;
    public FOVEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return EffectResponse.Failure(request.ID, "Main camera not found");
        _originalFov = cam.fieldOfView;
        switch (request.code)
        {
            case "lowfov": cam.fieldOfView = 40f; break; // Lower FOV (zoomed in)
            case "highfov": cam.fieldOfView = 90f; break; // Higher FOV (zoomed out)
            default: return EffectResponse.Failure(request.ID, "Unknown FOV effect");
        }
        return EffectResponse.Success(request.ID);
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        Camera cam = Camera.main;
        if (cam != null)
            cam.fieldOfView = _originalFov;
        return EffectResponse.Finished(request.ID);
    }
}
