namespace PRM_BE.DTO
{
    public class OrderDTO
    {
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
       
    }

    public class  UpdateOrderDTO
    {
        public int OrderId { get; set; }

        public string OrderStatus { get; set; } = string.Empty;
    }
}
