namespace CrowdControl.Delegates.Metadata;

/// <summary> Attribute used to mark a method as metadata with the specified IDs.</summary>
[AttributeUsage(AttributeTargets.Method)]
public class MetadataAttribute : Attribute
{
    /// <summary>
    /// Gets the IDs associated with the metadata.
    /// </summary>
    public string[] IDs { get; }

    public MetadataAttribute(string ids) : this(new[] { ids }) { }

#if NET7_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    public MetadataAttribute(params IEnumerable<string> ids) : this(ids.ToArray()) { }
#else
    public MetadataAttribute(IEnumerable<string> ids) : this(ids.Select(id => id).ToArray()) { }
    /// <summary> Attribute used to mark a method as metadata with the specified IDs.</summary>
    public MetadataAttribute(params string[] ids)
    {
        IDs = ids;
    }
#endif
}