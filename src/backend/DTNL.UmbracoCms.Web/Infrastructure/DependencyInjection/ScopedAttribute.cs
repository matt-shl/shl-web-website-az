using Scrutor;

namespace DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScopedAttribute : ServiceDescriptorAttribute
{
    public ScopedAttribute()
        : this(null)
    {
    }

    public ScopedAttribute(Type? serviceType)
        : base(serviceType, ServiceLifetime.Scoped)
    {
    }
}
