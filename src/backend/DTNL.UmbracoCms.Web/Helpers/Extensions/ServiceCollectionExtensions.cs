namespace DTNL.UmbracoCms.Web.Helpers.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Removes all services implemented by type <typeparamref name="TImpl"/> in <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="collection">The <see cref="IServiceCollection"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection RemoveAllImplementedBy<TService, TImpl>(this IServiceCollection collection)
    {
        collection.RemoveAll(descriptor => descriptor.GetImplementationType() == typeof(TImpl) && descriptor.ServiceType == typeof(TService));
        return collection;
    }

    private static Type? GetImplementationType(this ServiceDescriptor descriptor)
    {
        return descriptor.ImplementationType
               ?? descriptor.ImplementationInstance?.GetType()
               ?? descriptor.ImplementationFactory?.Method.ReturnType;
    }
}
