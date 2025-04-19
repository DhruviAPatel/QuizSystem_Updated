using StudentPortal.Web.Models;
using StudentPortal.Web.Models.Entities;
using System.Threading.Tasks;

public interface IAccountService
{
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task<bool> IsEmailExistsAsync(string email);
    Task<User?> RegisterUserAsync(RegisterViewModel model);


}
