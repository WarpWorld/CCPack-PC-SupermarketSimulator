using Il2Cpp;

namespace CrowdControl.Delegates.Effects.Implementations;

public abstract class BoxEffectBase : Effect
{
    protected BoxEffectBase(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }
    
    public static ProductSO GetProduct(string name)
    {
        foreach (int productID in ProductLicenseManager.Instance.AllPoducts)
        {
            ProductSO p = IDManager.Instance.ProductSO(productID);
            if (p.ProductName.ToUpper().Contains(name.ToUpper())) return p;
        }
        return null;
    }
}