using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Passport.Data;
using Passport.Models;
using Passport.Models.DTO;
using Passport.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Passport.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public UserRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<OperationResult> DeleteUser(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            if(user == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User not found"
                };
            }
            else
            {
                await _userManager.DeleteAsync(user);
                return new OperationResult
                {
                    Succeeded = true,
                    Message = "User deleted"
                };
            }
        }

        public string GenerateToken(Login login)
        {
            var user =  _userManager.FindByEmailAsync(login.Email!).Result;
            var roles = _userManager.GetRolesAsync(user!).Result;
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, login.Email!),
                new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var securityToken = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.Now.AddMinutes(60),
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value
                );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        public bool isUniqueUser(string name)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == name);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<OperationResult> Login(Login login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email!);
            if (user == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Invalid email or password"
                };
            }
            var result = await _signInManager.PasswordSignInAsync(user, login.Password!, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return new OperationResult
                {
                    Succeeded = true,
                    Message = "Login successful",
                    Data = user
                };
            }
            return new OperationResult
            {
                Succeeded = false,
                Message = "Invalid email or password",
                Data = user 
            };
        }

        //public Task<IActionResult> Logout()
        //{
        //    var logout = _signInManager.SignOutAsync();
        //    return ;
        //}

        public async Task<OperationResult> Register(RegisterDTO registerDTO)
        {
            var existUser = await _userManager.FindByEmailAsync(registerDTO.Email!);
            if (existUser != null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User already exists",
                };
            }
            ApplicationUser applicationUser = new ApplicationUser()
            {
                PersonName = registerDTO.Name,
                Email = registerDTO.Email,
                UserName = registerDTO.Email
                
            };
            IdentityResult result = await _userManager.CreateAsync(applicationUser, registerDTO.Password!);
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(applicationUser);
                var check = await _userManager.FindByEmailAsync(applicationUser.Email!);
                if(check!.Email == "administratorsite@gmail.com")
                {
                    var role = await _roleManager.CreateAsync(new ApplicationRole(){ Name = "Admin"});
                    await _userManager.AddToRoleAsync(check, "Admin");
                }
                else
                {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = "User"});
                    await _userManager.AddToRoleAsync(check, "User");
                }
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                return new OperationResult
                {
                    Succeeded = true,
                    Message = "Registration successful",
                    Data = result,
                    Errors = new List<string>()
                };
            }
            else
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }
        }
    }
}
