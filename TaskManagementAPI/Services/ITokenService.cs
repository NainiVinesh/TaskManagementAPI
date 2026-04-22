using TaskManagementAPI.Models;
namespace TaskManagementAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
