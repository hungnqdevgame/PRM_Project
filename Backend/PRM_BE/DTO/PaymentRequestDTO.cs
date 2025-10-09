namespace PRM_BE.DTO
{
    public class PaymentRequestDTO
    {
       public int UserId { get; set; }
        public int OrderId { get; set; }
      
        public int Amount { get; set; } 
    }
}
