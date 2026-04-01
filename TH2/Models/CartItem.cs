namespace TH2.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        // Tính tổng tiền của sản phẩm này
        public decimal Total => Price * Quantity;
    }
}