using System;
using ConnectorLib.JSON;
using Il2Cpp;

namespace CrowdControl.Delegates.Effects.Implementations;

[Effect("setlanguage_english", "setlanguage_french", "setlanguage_german", "setlanguage_italiano", "setlanguage_espanol", "setlanguage_portugal", "setlanguage_brazil", "setlanguage_nederlands", "setlanguage_turkce")]
public class LanguageEffects : Effect
{
    private bool _changed;
    public LanguageEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            SaveManager save = SaveManager.Instance; if (save==null) return EffectResponse.Retry(request.ID);
            int current = save.Settings.LanguageSetting;
            int newLang = current;
            switch(request.code)
            {
                case "setlanguage_english": newLang = 0; break;
                case "setlanguage_french": newLang = 1; break;
                case "setlanguage_german": newLang = 2; break;
                case "setlanguage_italiano": newLang = 3; break;
                case "setlanguage_espanol": newLang = 4; break;
                case "setlanguage_portugal": newLang = 5; break;
                case "setlanguage_brazil": newLang = 6; break;
                case "setlanguage_nederlands": newLang = 7; break;
                case "setlanguage_turkce": newLang = 8; break;
            }
            if (newLang == current) return EffectResponse.Failure(request.ID, "Already that language");
            GameStateManager.OrgLanguage = current;
            GameStateManager.NewLanguage = newLang;
            save.Settings.LanguageSetting = newLang;
            _changed = true;
            return EffectResponse.Success(request.ID);
        }
        catch(Exception e){CrowdControlMod.Instance.Logger.Error($"Language error: {e}"); return EffectResponse.Retry(request.ID);}        
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        if (_changed)
        {
            try{ SaveManager save = SaveManager.Instance; save.Settings.LanguageSetting = GameStateManager.OrgLanguage; }catch{}
        }
        return EffectResponse.Finished(request.ID);
    }
}