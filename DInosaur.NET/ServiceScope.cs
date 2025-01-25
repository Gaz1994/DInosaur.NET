using DInosaur.NET.Context;

namespace DInosaur.NET;

public class ServiceScope : IDisposable
{
    private readonly ServiceContext _context;
    
    public ServiceScope()
    {
        _context = new ServiceContext();
        ServiceContext.Current.Value = _context;
    }

    public void Dispose()
    {
        ServiceContext.Current.Value = null;
    }
}