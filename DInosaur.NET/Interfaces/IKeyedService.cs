namespace DInosaur.NET.Interfaces;

public interface IKeyedService : IServiceLifetime
{
    static abstract object ServiceKey { get; }
}
