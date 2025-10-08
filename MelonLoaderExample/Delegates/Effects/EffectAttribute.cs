namespace CrowdControl.Delegates.Effects;

/// <summary>Attribute used to mark a method for starting an effect with the specified ID(s).</summary>
[AttributeUsage(AttributeTargets.Class)]
public class EffectAttribute : Attribute
{
    /// <summary>
    /// Gets the IDs associated with the effect.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public IReadOnlyList<string> IDs { get; }

    /// <summary>
    /// The duration of the effect, if applicable.
    /// </summary>
    public SITimeSpan DefaultDuration { get; }

    /// <summary>
    /// All conflicting effect IDs, if applicable.
    /// </summary>
    public IReadOnlyList<string> Conflicts { get; }

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public EffectAttribute(params IEnumerable<string> ids) : this(ids.ToArray(), SITimeSpan.Zero, Array.Empty<string>) { }
#else
    public EffectAttribute(IEnumerable<string> ids) : this(ids.ToArray(), SITimeSpan.Zero, Array.Empty<string>()) { }
#endif

    public EffectAttribute(params string[] ids) : this(ids.ToArray(), SITimeSpan.Zero, Array.Empty<string>()) { }

    public EffectAttribute(string[] ids, float defaultDuration, string[] conflicts) : this(ids, (SITimeSpan)defaultDuration, conflicts) { }
    
    public EffectAttribute(string[] ids, float defaultDuration, string conflict) : this(ids, defaultDuration, new[] { conflict }) { }

    public EffectAttribute(string id) : this(new[] { id }, SITimeSpan.Zero, Array.Empty<string>()) { }

    public EffectAttribute(string id, float defaultDuration) : this(new[] { id }, defaultDuration, (SITimeSpan.Zero > 0) ? new[] { id } : Array.Empty<string>()) { }

    public EffectAttribute(string id, float defaultDuration, string conflict) : this(new[] { id }, defaultDuration, new[] { conflict }) { }

    public EffectAttribute(string id, float defaultDuration, string[] conflicts) : this(new[] { id }, defaultDuration, conflicts) { }

    public EffectAttribute(string id, float defaultDuration, bool selfConflict) : this(new[] { id }, defaultDuration, selfConflict ? new[] { id } : Array.Empty<string>()) { }

    public EffectAttribute(string[] ids, float defaultDuration, bool selfConflict) : this(ids, defaultDuration, selfConflict ? ids : Array.Empty<string>()) { }

    public EffectAttribute(string id, bool selfConflict) : this(new[] { id }, SITimeSpan.Zero, selfConflict ? new[] { id } : Array.Empty<string>()) { }

    public EffectAttribute(string[] ids, bool selfConflict) : this(ids, SITimeSpan.Zero, selfConflict ? ids : Array.Empty<string>()) { }

    /// <summary>Attribute used to mark a method for starting an effect with the specified ID(s).</summary>
    public EffectAttribute(string[] ids, SITimeSpan defaultDuration, string[] conflicts)
    {
        IDs = ids;
        DefaultDuration = defaultDuration;
        Conflicts = conflicts;
    }
}