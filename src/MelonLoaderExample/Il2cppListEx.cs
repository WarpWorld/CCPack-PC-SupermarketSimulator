//do not add using directives here, it makes the usage visually unclear without the full namespaces

namespace CrowdControl;

public static class Il2cppListEx
{
    public static TValue FirstOrDefault<TValue>(this Il2CppSystem.Collections.Generic.List<TValue> list)
    {
        if (list == null || list.Count == 0) return default;
        return list[0];
    }
    
    public static TValue FirstOrDefault<TValue>(this Il2CppSystem.Collections.Generic.List<TValue> list, System.Func<TValue, bool> predicate)
    {
        if (list == null || list.Count == 0) return default;
        foreach (TValue item in list)
            if (predicate(item))
                return item;
        return default;
    }
}