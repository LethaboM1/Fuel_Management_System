namespace FMS.Models
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Station { get; set; }
        public string Password { get; set; }
    }
}