// 1. Only interface injection

using DInosaur.NET.Attributes;
using DInosaur.NET.Interfaces;

public interface IUserService : ITransient
{
    void ProcessUser();
}
public class UserService : IUserService
{
    public void ProcessUser() => Console.WriteLine("Process user");
}

// 2. Only class injection
public interface IOrderService
{
    void ProcessOrder();
}
public class OrderService : IOrderService, ITransient
{
    public void ProcessOrder() => Console.WriteLine("Process order");
}


// 3. Class attribute
public interface IPaymentService
{
    void ProcessPayment();
}
[Transient]
public class PaymentService : IPaymentService
{
    public void ProcessPayment() => Console.WriteLine("Process payment");
}