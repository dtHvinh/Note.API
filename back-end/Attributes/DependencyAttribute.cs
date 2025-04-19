namespace back_end.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DependencyAttribute : Attribute
{
    public Type? BaseType { get; set; }
    public ServiceLifetime Lifetime { get; set; }
}
