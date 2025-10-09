namespace PRM_BE.DTO
{
    public class OrderInfoModel
    {
        public string UserName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
    }
}
