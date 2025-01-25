using DInosaur.NET.Interfaces;

namespace DInosaur.NET.Attributes;

// Lifetime attributes
[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    public Type LifetimeType { get; }

    protected ServiceLifetimeAttribute(Type lifetimeType)
    {
        if (!typeof(IServiceLifetime).IsAssignableFrom(lifetimeType))
            throw new ArgumentException($"Type must implement {nameof(IServiceLifetime)}", nameof(lifetimeType));
        LifetimeType = lifetimeType;
    }
}

public class SingletonAttribute() : ServiceLifetimeAttribute(typeof(ISingleton));
public class TransientAttribute() : ServiceLifetimeAttribute(typeof(ITransient));

public class ScopedAttribute() : ServiceLifetimeAttribute(typeof(IScoped));

public class ThreadScopedAttribute() : ServiceLifetimeAttribute(typeof(IThreadScoped));

public class PooledAttribute() : ServiceLifetimeAttribute(typeof(IPooled));