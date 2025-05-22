using System;
using System.Collections.Generic;
using System.Linq;

public class OrderEventArgs : EventArgs
{
    public Order Order { get; set; }
    public string NewStatus { get; set; }

    public OrderEventArgs(Order order, string status)
    {
        Order = order;
        NewStatus = status;
    }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; private set; }

    public event EventHandler<OrderEventArgs> OrderStatusChanged;

    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
        OnOrderStatusChanged(new OrderEventArgs(this, newStatus));
    }

    protected virtual void OnOrderStatusChanged(OrderEventArgs e)
    {
        OrderStatusChanged?.Invoke(this, e);
    }
}

public class Kitchen
{
    public void Subscribe(Order order)
    {
        order.OrderStatusChanged += (s, e) =>
        {
            if (e.NewStatus == "Moi")
                Console.WriteLine($"[Kitchen] Chuan bi đon #{e.Order.Id} cho {e.Order.CustomerName}.");
        };
    }
}

public class Delivery
{
    public void Subscribe(Order order)
    {
        order.OrderStatusChanged += (s, e) =>
        {
            if (e.NewStatus == "Dang giao")
                Console.WriteLine($"[Delivery] Đon #{e.Order.Id} đa san sang de giao.");
        };
    }
}

public class CustomerService
{
    public void Subscribe(Order order)
    {
        order.OrderStatusChanged += (s, e) =>
        {
            if (e.NewStatus == "Huy" || e.NewStatus == "Giao that bai")
                Console.WriteLine($"[CSKH] Lien he khach ve đon #{e.Order.Id}: {e.NewStatus}.");
        };
    }
}

class Program
{
    static void Main()
    {
        var orders = new List<Order>
        {
            new Order { Id = 1, CustomerName = "Nam" },
            new Order { Id = 2, CustomerName = "Linh" },
            new Order { Id = 3, CustomerName = "Tu" }
        };

        var kitchen = new Kitchen();
        var delivery = new Delivery();
        var cskh = new CustomerService();

        foreach (var order in orders)
        {
            kitchen.Subscribe(order);
            delivery.Subscribe(order);
            cskh.Subscribe(order);
        }

        Predicate<Order> isDelivering = o => o.Status == "Dang giao";
        Func<Order, string> format = o => $"Don #{o.Id} ({o.CustomerName}) - {o.Status}";
        Action<string> logger = msg => Console.WriteLine($"[LOG] {msg}");

        orders[0].UpdateStatus("Moi");
        orders[0].UpdateStatus("Dang giao");
        orders[0].UpdateStatus("Hoan tat");

        orders[1].UpdateStatus("Moi");
        orders[1].UpdateStatus("Huy");

        orders[2].UpdateStatus("Moi");
        orders[2].UpdateStatus("Dang giao");
        orders[2].UpdateStatus("Giao that bai");

        foreach (var order in orders)
        {
            logger(format(order));
        }

        var thanhCong = orders.Count(o => o.Status == "Hoan tat");
        var huy = orders.Count(o => o.Status == "Huy");

        Console.WriteLine($"\nThong ke:");
        Console.WriteLine($"- Don giao thanh cong: {thanhCong}");
        Console.WriteLine($"- Don bi huy: {huy}");
    }
}
