namespace ShopDev.Authentication.ApplicationServices.AuthenticationModule.Dtos.UserDto
{
    public class UpdateFullNameUserDto
    {
        public int Id { get; set; }

        private string? _fullName;
        public string? FullName
        {
            get => _fullName;
            set => _fullName = value?.Trim();
        }
    }
}
