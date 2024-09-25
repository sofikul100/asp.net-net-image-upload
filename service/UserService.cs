using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(UserCreateDto userCreateDto);
    Task<User> UpdateUserAsync(int id, UserCreateDto userUpdateDto);
    Task<bool> DeleteUserAsync(int id);
}


public class UserService : IUserService
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _context; // Your DbContext to interact with the database
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserService(AppDbContext context, IWebHostEnvironment env,IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _env = env;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> CreateUserAsync(UserCreateDto userCreateDto)
    {
        // Handle image file saving
        string imagePath = await SaveImage(userCreateDto.Image);

        var user = new User
        {
            Name = userCreateDto.Name,
            Email = userCreateDto.Email,
            ImagePath = imagePath
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(int id, UserCreateDto userUpdateDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return null;

        // Update image if a new one is uploaded
        if (userUpdateDto.Image != null)
        {
            // Delete the old image
            if (!string.IsNullOrEmpty(user.ImagePath))
                DeleteImage(user.ImagePath);

            // Save the new image
            user.ImagePath = await SaveImage(userUpdateDto.Image);
        }

        user.Name = userUpdateDto.Name;
        user.Email = userUpdateDto.Email;

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        // Delete the image file
        if (!string.IsNullOrEmpty(user.ImagePath))
            DeleteImage(user.ImagePath);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper method to save the image
   public async Task<string> SaveImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("Invalid image file");
        }

        // Ensure the path to store images is not null
        string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        // Generate a unique file name for the image
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
        string filePath = Path.Combine(uploadFolder, uniqueFileName);

        // Save the image
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return uniqueFileName;
    }

    // Helper method to delete the image
    private void DeleteImage(string imagePath)
    {
        var filePath = Path.Combine(_env.WebRootPath, "images", imagePath);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
