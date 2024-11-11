using Scrutor;

namespace DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TransientAttribute : ServiceDescriptorAttribute
{
    public TransientAttribute()
        : this(null)
    {
    }

    public TransientAttribute(Type? serviceType)
        : base(serviceType, ServiceLifetime.Transient)
    {
    }
}
