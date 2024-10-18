namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class PrivacyInfoDto
    {
        public DateTime? LastLogin { get; set; }
        public string? OperatingSystem { get; set; }
        public string? Browser { get; set; }
        public string? AvatarImageUri { get; set; }
    }
}
