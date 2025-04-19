using Microsoft.EntityFrameworkCore;
using StudentPortal.Web.Data;
using StudentPortal.Web.Models.Entities;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.UsersData.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> CreateUserAsync(User user)
    {
        await _context.UsersData.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
