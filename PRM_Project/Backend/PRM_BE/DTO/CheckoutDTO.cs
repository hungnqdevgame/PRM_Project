namespace PRM_BE.DTO
{
    public class CheckoutDTO
    {
        public int Amount { get; set; }
        public string SupscriptionName { get; set; }
        //public int Amount { get; set; }
        public string CallbackUrl { get; set; }
        public string ReturnUrl { get; set; }
    }
}
