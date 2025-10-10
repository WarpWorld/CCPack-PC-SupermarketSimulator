using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppDG.Tweening;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[] {"lowfov","highfov"}, 30, new[]{"lowfov","highfov"})]
public class FOVEffects : Effect
{
    public FOVEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        Camera cam = Camera.main;
        if (cam == null)
            return EffectResponse.Failure(request.ID, "Main camera not found");
        
        switch (request.code)
        {
            case "lowfov": cam.DOFieldOfView(30f, 0.5f); break; // Lower FOV (zoomed in)
            case "highfov": cam.DOFieldOfView(140f, 0.5f); break; // Higher FOV (zoomed out)
            default: return EffectResponse.Failure(request.ID, "Unknown FOV effect");
        }
        
        return EffectResponse.Success(request.ID);
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        Camera cam = Camera.main;
        if (cam != null)
            cam.DOFieldOfView(SaveManager.Instance.Settings.FOV, 0.5f);
        return EffectResponse.Finished(request.ID);
    }
}
