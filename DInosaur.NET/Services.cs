using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using DInosaur.NET;
using DInosaur.NET.Attributes;
using DInosaur.NET.Context;
using DInosaur.NET.Interfaces;

public static class Services
{
    private static readonly ConcurrentDictionary<Type, object> Singletons = new();
    private static readonly ConcurrentDictionary<Type, ObjectPool<object>> Pools = new();
    private static readonly ConcurrentDictionary<(Type, object), object> KeyedServices = new();
    private static readonly ThreadLocal<ConcurrentDictionary<Type, object>> ThreadScoped = 
        new(() => new ConcurrentDictionary<Type, object>());
    private static readonly ConcurrentDictionary<Type, Type> ImplementationTypes = new();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Get<T>() where T : class => (T)GetService(typeof(T));
    private static object GetService(Type serviceType)
    {
        var lifetimeType = GetLifetimeType(serviceType);

        if (lifetimeType == null)
            throw new InvalidOperationException($"No lifetime found for {serviceType}");

        return lifetimeType.IsAssignableTo(typeof(ITransient)) ? CreateInstance(serviceType)
            : lifetimeType.IsAssignableTo(typeof(ISingleton)) ? Singletons.GetOrAdd(serviceType, CreateInstance)
            : lifetimeType.IsAssignableTo(typeof(IScoped)) ? ServiceContext.Current.Value?.GetOrCreateScoped(() => CreateInstance(serviceType)) 
                                                             ?? throw new InvalidOperationException("No active scope")
            : lifetimeType.IsAssignableTo(typeof(IThreadScoped)) ? ThreadScoped.Value!.GetOrAdd(serviceType, CreateInstance)
            : lifetimeType.IsAssignableTo(typeof(IPooled)) ? GetOrCreatePool(serviceType).Get()
            : lifetimeType.IsAssignableTo(typeof(IKeyedService)) ? KeyedServices.GetOrAdd((serviceType, GetServiceKey(serviceType)), 
                _ => CreateInstance(serviceType))
            : throw new InvalidOperationException($"Unknown lifetime type for {serviceType}");
    }
    
    private static Type? GetLifetimeType(Type serviceType)
    {
        // If it's an interface, find implementation first
        if (serviceType.IsInterface)
        {
            var implementationType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .First(t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(serviceType));
            
            // Check implementation's lifetime
            var implLifetime = implementationType.GetInterfaces()
                .FirstOrDefault(i => i.IsAssignableTo(typeof(IServiceLifetime)));
            if (implLifetime != null) return implLifetime;
            
            // Check implementation's attributes
            var attrLifetime = implementationType.GetCustomAttribute<ServiceLifetimeAttribute>()?.LifetimeType;
            if (attrLifetime != null) return attrLifetime;
        }

        // Check interface lifetime (for interface injection)
        var interfaceLifetime = serviceType.GetInterfaces()
            .FirstOrDefault(i => i.IsAssignableTo(typeof(IServiceLifetime)));
        if (interfaceLifetime != null) 
            return interfaceLifetime;

        // Check attributes
        return serviceType.GetCustomAttribute<ServiceLifetimeAttribute>()?.LifetimeType;
    }
    private static object GetServiceKey(Type serviceType)
    {
        var property = serviceType.GetProperty("ServiceKey", 
            BindingFlags.Public | BindingFlags.Static);
        return property?.GetValue(null) 
            ?? throw new InvalidOperationException($"ServiceKey not found for {serviceType}");
    }

    private static ObjectPool<object> GetOrCreatePool(Type serviceType)
    {
        return Pools.GetOrAdd(serviceType, t =>
        {
            var property = t.GetProperty("PoolSize",
                BindingFlags.Public | BindingFlags.Static);
            var poolSize = (int)(property?.GetValue(null) ?? 10);
            return new ObjectPool<object>(() => CreateInstance(t), poolSize);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object CreateInstance(Type type)
    {
        if (type.IsInterface)
        {
            type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .First(t => !t.IsInterface && !t.IsAbstract && t.IsAssignableTo(type));
        }

        var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
    
        // If no public constructors, try to create instance directly
        if (!constructors.Any())
            return Activator.CreateInstance(type)!;
        
        var ctor = constructors.MaxBy(c => c.GetParameters().Length)!;
        var parameters = ctor.GetParameters()
            .Select(p => GetService(p.ParameterType))
            .ToArray();

        return ctor.Invoke(parameters);
    }
}