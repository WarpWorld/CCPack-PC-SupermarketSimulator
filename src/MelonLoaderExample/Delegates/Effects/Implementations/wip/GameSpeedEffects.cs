using ConnectorLib.JSON;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[] {"ultraslow","slow","fast","ultrafast"}, 0, new[]{"ultraslow","slow","fast","ultrafast"})]
public class GameSpeedEffects : Effect
{
    private float _originalScale = 1f;
    public GameSpeedEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        _originalScale = Time.timeScale;
        switch (request.code)
        {
            case "ultraslow": Time.timeScale = 0.1f; break; // Keep legacy style (very slow)
            case "slow": Time.timeScale = 0.5f; break;
            case "fast": Time.timeScale = 1.5f; break;
            case "ultrafast": Time.timeScale = 2.0f; break;
            default: return EffectResponse.Failure(request.ID, "Unknown speed effect");
        }
        return EffectResponse.Success(request.ID);
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        Time.timeScale = _originalScale;
        return EffectResponse.Finished(request.ID);
    }
}

