using ConnectorLib.JSON;
using Il2CppAssets.Scripts.Actors.Player;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;


[Effect(
    ids: new[] { "slowTime", "speedUpTime" },
    defaultDuration: 15,
    conflicts: new[] { "slowTime", "speedUpTime" }
)]
public class TimeManipulation : Effect
{
    public TimeManipulation(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private float _previousTimeScale = 1.0f;

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            _previousTimeScale = Time.timeScale;

            switch (request.code)
            {
                case "slowTime":
                    Time.timeScale = 0.5f; // Half speed
                    break;

                case "speedUpTime":
                    Time.timeScale = 2.0f; // Double speed
                    break;

                default:
                    return EffectResponse.Failure(request.ID, $"Unknown effect code {request.code}");
            }

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Time manipulation start error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        try
        {
            Time.timeScale = _previousTimeScale;
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Time manipulation stop error: {e}");
        }

        return EffectResponse.Finished(request.ID);
    }
}