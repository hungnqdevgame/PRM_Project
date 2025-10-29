namespace PRM_BE.DTO
{
    public class StoreLocationDTO
    {
        public string Id { get; set; }
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Address { get; set; } = null!;

    }
}
