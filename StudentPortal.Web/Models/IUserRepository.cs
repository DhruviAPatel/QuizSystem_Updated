using StudentPortal.Web.Models.Entities;
using System.Threading.Tasks;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> CreateUserAsync(User user);
}
