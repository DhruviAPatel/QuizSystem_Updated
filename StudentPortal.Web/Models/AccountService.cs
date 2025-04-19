using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;


public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;


    private readonly ApplicationDbContext _context;

    public AccountService(ApplicationDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || user.Password != password)
        {
            return null;
        }
        return user;
    }

    public async Task<User?> RegisterUserAsync(RegisterViewModel model)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
        if (existingUser != null) return null;

        var newUser = new User
        {
            Email = model.Email,
            Password = model.Password,
            Role = model.Role
        };

        return await _userRepository.CreateUserAsync(newUser);
    }
    public async Task<bool> IsEmailExistsAsync(string email)
    {
        return await _context.UsersData.AnyAsync(u => u.Email == email);
    }

}
