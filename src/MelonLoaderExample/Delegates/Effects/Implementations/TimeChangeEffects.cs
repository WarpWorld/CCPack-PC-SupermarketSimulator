using System;
using ConnectorLib.JSON;
using Il2Cpp;
using Action = Il2CppSystem.Action;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("plushour", "minushour")]
public class TimeChangeEffects : Effect
{
    public TimeChangeEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }
    
    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            DayCycleManager dm = DayCycleManager.Instance;
            if (dm == null) return EffectResponse.Retry(request.ID);
            if (!StoreStatus.Instance.IsOpen) return EffectResponse.Retry(request.ID);

            bool add = request.code == "plushour";
            int hour = dm.m_CurrentTimeInHours;
            bool am = dm.m_AM;

            if (add)
            {
                if (hour >= 8 && !am) return EffectResponse.Retry(request.ID); // matches legacy gating
                float dayDur = dm.m_DayDurationInGameTimeInSeconds;
                float curFloat = dm.m_CurrentTimeInFloat + 3600f;
                dm.m_CurrentTimeInFloat = curFloat;
                SaveManager save = SaveManager.Instance;
                save.Progression.CurrentTime += 3600f;
                dm.m_DayPercentage = save.Progression.CurrentTime / dayDur;
                dm.UpdateGameTime();
                dm.UpdateLighting();
            }
            else
            {
                if ((hour < 9 && am) || (hour >= 9 && !am)) return EffectResponse.Retry(request.ID);
                SaveManager save = SaveManager.Instance;
                float dayDur = dm.m_DayDurationInGameTimeInSeconds;
                save.Progression.CurrentTime -= 3600f;
                dm.m_DayPercentage = save.Progression.CurrentTime / dayDur;
                if (hour == 1) hour = 12; else if (hour == 12)
                {
                    hour = 11;
                    dm.m_AM = true;
                }
                else hour--;
                dm.m_CurrentTimeInHours = hour;
                // trigger events
                Action onFull = dm.OnFullHour; onFull?.Invoke();
                Action onTime = dm.OnTimeChanged; onTime?.Invoke();
                dm.UpdateLighting();
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"HourAdjust error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}