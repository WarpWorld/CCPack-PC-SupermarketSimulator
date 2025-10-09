using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect(new[]{"plushour","minushour"})]
public class HourAdjustEffects : Effect
{
    public HourAdjustEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private static object GetField(object obj,string name){return obj.GetType().GetField(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.GetValue(obj);}    
    private static void SetField(object obj,string name,object val){obj.GetType().GetField(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic)?.SetValue(obj,val);}    
    private static void Call(object obj,string name){obj.GetType().GetMethod(name,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Public)?.Invoke(obj,null);}    

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var dm = Singleton<DayCycleManager>.Instance; if (dm==null) return EffectResponse.Retry(request.ID);
            if (!Singleton<StoreStatus>.Instance.IsOpen) return EffectResponse.Retry(request.ID);

            bool add = request.code == "plushour";
            int hour = (int)GetField(dm,"m_CurrentTimeInHours");
            bool am = (bool)GetField(dm,"m_AM");

            if (add)
            {
                if (hour >= 8 && !am) return EffectResponse.Retry(request.ID); // matches legacy gating
                float dayDur = (float)GetField(dm,"m_DayDurationInGameTimeInSeconds");
                float curFloat = (float)GetField(dm,"m_CurrentTimeInFloat") + 3600f;
                SetField(dm,"m_CurrentTimeInFloat",curFloat);
                var save = Singleton<SaveManager>.Instance;
                save.Progression.CurrentTime += 3600f;
                SetField(dm,"m_DayPercentage", save.Progression.CurrentTime/dayDur);
                Call(dm,"UpdateGameTime");
                Call(dm,"UpdateLighting");
            }
            else
            {
                if ((hour < 9 && am) || (hour >= 9 && !am)) return EffectResponse.Retry(request.ID);
                var save = Singleton<SaveManager>.Instance;
                float dayDur = (float)GetField(dm,"m_DayDurationInGameTimeInSeconds");
                save.Progression.CurrentTime -= 3600f;
                SetField(dm,"m_DayPercentage", save.Progression.CurrentTime/dayDur);
                if (hour == 1) hour = 12; else if (hour == 12){ hour = 11; SetField(dm,"m_AM", true);} else hour--;
                SetField(dm,"m_CurrentTimeInHours", hour);
                // trigger events
                var onFull = dm.OnFullHour; onFull?.Invoke();
                var onTime = dm.OnTimeChanged; onTime?.Invoke();
                Call(dm,"UpdateLighting");
            }
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"HourAdjust error: {e}"); return EffectResponse.Retry(request.ID);}        
    }
}