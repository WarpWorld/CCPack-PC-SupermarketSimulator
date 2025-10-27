using System;
using ConnectorLib.JSON;
using Il2Cpp;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("invertx", "inverty")]
public class InvertAxisEffects : Effect
{
    public InvertAxisEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            SaveManager saveManager = SaveManager.Instance;
            SettingsMenuManager settingsMenuManager = UnityEngine.Object.FindObjectOfType<SettingsMenuManager>();
            if (request.code == "invertx")
                settingsMenuManager.InvertXAxis(!saveManager.Settings.InvertXAxis);
            else
                settingsMenuManager.InvertYAxis(!saveManager.Settings.InvertYAxis);

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"InvertAxis error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        try
        {
            SaveManager saveManager = SaveManager.Instance;
            SettingsMenuManager settingsMenuManager = UnityEngine.Object.FindObjectOfType<SettingsMenuManager>();

            if (request.code == "invertx")
                settingsMenuManager.InvertXAxis(!saveManager.Settings.InvertXAxis);
            else
                settingsMenuManager.InvertYAxis(!saveManager.Settings.InvertYAxis);

            return EffectResponse.Finished(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"InvertAxis error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }
}