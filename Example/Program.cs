// See https://aka.ms/new-console-template for more information

using DInosaur.NET;

Console.WriteLine("Hello, World!");

// Program.cs
var user = Services.Get<IUserService>();
var order = Services.Get<IOrderService>();
var payment = Services.Get<IPaymentService>();

user.ProcessUser();
order.ProcessOrder(); 
payment.ProcessPayment();