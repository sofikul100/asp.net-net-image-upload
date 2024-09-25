public class UserCreateDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public IFormFile Image { get; set; } // Image for file upload
}
