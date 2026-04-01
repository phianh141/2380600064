namespace TH2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; } // Chứa ID của người dùng đã đăng nhập
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Pending"; // Pending, Shipping, Completed, Cancelled
        public string CustomerName { get; set; } = null!;
        public string? ShippingAddress { get; set; }
        public string? PhoneNumber { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }
    }
}