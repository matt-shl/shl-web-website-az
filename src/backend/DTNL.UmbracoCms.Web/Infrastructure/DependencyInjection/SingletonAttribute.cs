using Scrutor;

namespace DTNL.UmbracoCms.Web.Infrastructure.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SingletonAttribute : ServiceDescriptorAttribute
{
    public SingletonAttribute()
        : this(null)
    {
    }

    public SingletonAttribute(Type? serviceType)
        : base(serviceType, ServiceLifetime.Singleton)
    {
    }
}
