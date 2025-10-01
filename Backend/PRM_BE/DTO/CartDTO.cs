namespace PRM_BE.DTO
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public List<CartItemDTO> Items { get; set; } = new();
    }
}
