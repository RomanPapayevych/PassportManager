using Microsoft.AspNetCore.Mvc;
using Passport.Models;
using Passport.Models.DTO;

namespace Passport.Repository.IRepository
{
    public interface IUserRepository
    {
        bool isUniqueUser(string name);
        string GenerateToken(Login login);
        Task<OperationResult> Register(RegisterDTO registerDTO);
        Task<OperationResult> Login(Login login);
        Task<OperationResult> DeleteUser(string name);
        //IActionResult Logout();
    }
}
