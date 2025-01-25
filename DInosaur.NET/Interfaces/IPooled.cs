namespace DInosaur.NET.Interfaces;

public interface IPooled : IServiceLifetime
{
    static abstract int PoolSize { get; }
}