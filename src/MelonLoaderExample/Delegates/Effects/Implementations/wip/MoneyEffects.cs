using ConnectorLib.JSON;
using Il2Cpp;
using Il2CppPG;

namespace CrowdControl.Delegates.Effects.Implementations;

// Handles give/take money effects (legacy: Money100 / MoneyN100 etc)
[Effect(new[] { "money100", "money1000", "money10000", "money-100", "money-1000", "money-10000" })]
public class MoneyEffects : Effect
{
    public MoneyEffects(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            var moneyManager = Singleton<MoneyManager>.Instance;
            if (moneyManager == null)
                return EffectResponse.Retry(request.ID, "Money manager not ready");

            float amount = request.code switch
            {
                "money100" => 100f,
                "money1000" => 1000f,
                "money10000" => 10000f,
                "money-100" => -100f,
                "money-1000" => -1000f,
                "money-10000" => -10000f,
                _ => 0f
            };

            if (amount == 0f)
                return EffectResponse.Failure(request.ID, "Unknown money effect");

            if (amount < 0f)
            {
                if (!moneyManager.HasMoney(-amount))
                    return EffectResponse.Failure(request.ID, "Player lacks funds");
            }

            // TransitionType chosen to mirror legacy usage (CHECKOUT_INCOME)
            moneyManager.MoneyTransition(amount, MoneyManager.TransitionType.CHECKOUT_INCOME, true);
            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"MoneyEffects error: {e}");
            return EffectResponse.Retry(request.ID, e.Message);
        }
    }
}