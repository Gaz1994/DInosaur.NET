using System.Collections.Concurrent;
using System.Collections.Frozen;

namespace DInosaur.NET.Context;

// Service resolution context
public class ServiceContext
{
    internal static AsyncLocal<ServiceContext?> Current { get; } = new();
    private ConcurrentDictionary<Type, object> _scopedServices;

    public ServiceContext()
    {
        _scopedServices = new ConcurrentDictionary<Type, object>();
    }

    public T GetOrCreateScoped<T>(Func<T> factory) where T : class
    {
        return (T)_scopedServices.GetOrAdd(typeof(T), _ => factory()!);
    }
}
