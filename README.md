# 🦖 Dinosaur.NET

A lightweight, fast & easy dependency injection framework for .NET 9 that supports multiple registration patterns with zero configuration required!


## Features

- Zero configuration required
- Multiple service registration patterns
- Support for different lifetimes (Transient, Singleton, Scoped, etc.)
- Automatic constructor injection

## Installation

update coming soon... 

## Usage

### Service Registration Patterns

There are multiple ways to register your services:

1. **Interface Injection**
```csharp
public interface IUserService : ITransient
{
    void ProcessUser();
}

public class UserService : IUserService
{
    public void ProcessUser() => Console.WriteLine("Processing user");
}
```

2. **Class Implementation**
```csharp
public interface IOrderService
{
    void ProcessOrder();
}

public class OrderService : IOrderService, ITransient
{
    public void ProcessOrder() => Console.WriteLine("Processing order");
}
```

3. **Attribute-based**
```csharp
public interface IPaymentService
{
    void ProcessPayment();
}

[Transient]
public class PaymentService : IPaymentService
{
    public void ProcessPayment() => Console.WriteLine("Processing payment");
}
```

### Service Resolution

```csharp
// Get your service instance
var userService = Services.Get<IUserService>();
var orderService = Services.Get<IOrderService>();
var paymentService = Services.Get<IPaymentService>();
```

### Supported Lifetimes

- `ITransient` - New instance created each time
- `ISingleton` - Single instance shared across the application
- `IScoped` - Instance shared within a scope
- `IThreadScoped` - Instance shared within the current thread
- `IPooled` - Instances managed by an object pool
- `IKeyedService` - Multiple implementations with different keys

### Constructor Injection

Dinosaur.NET automatically handles constructor injection:

```csharp
public class OrderProcessor : ITransient
{
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;

    public OrderProcessor(IPaymentService paymentService, IUserService userService)
    {
        _paymentService = paymentService;
        _userService = userService;
    }
}
```

### Scoped Services

```csharp
using (Services.CreateScope())
{
    var scopedService = Services.Get<IScopedService>();
    // Service instance disposed when scope ends
}
```

## License

MIT
