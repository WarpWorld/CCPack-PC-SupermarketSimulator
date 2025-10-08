using ConnectorLib.JSON;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"invertx","inverty"})]
public class InvertAxisEffects : Effect
{
    public InvertAxisEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            if (request.code=="invertx")
            {
                if (TimedThread.isRunning(TimedType.INVERT_X)) return EffectResponse.Retry(request.ID);
                int durMs = (int)((request.duration>0? request.duration:30000));
                new Thread(new TimedThread(request.ID, TimedType.INVERT_X, durMs).Run).Start();
            }
            else
            {
                if (TimedThread.isRunning(TimedType.INVERT_Y)) return EffectResponse.Retry(request.ID);
                int durMs = (int)((request.duration>0? request.duration:30000));
                new Thread(new TimedThread(request.ID, TimedType.INVERT_Y, durMs).Run).Start();
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"InvertAxis error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}