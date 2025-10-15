namespace PRM_BE.DTO
{
    public class LoginResponseDTO
    {
        public string? AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }

        public string? Username { get; set; } = string.Empty;
    }
}
