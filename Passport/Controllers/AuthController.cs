using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Passport.Models;
using Passport.Models.DTO;
using Passport.Repository.IRepository;

namespace Passport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AuthController(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]Login login)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Login(login);
                if (user.Succeeded)
                {
                    var tokenString = _userRepository.GenerateToken(login);
                    return Ok(tokenString);
                }
                //return Ok(user!);
            }   
            return BadRequest("Wrong");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                bool ifUnique = _userRepository.isUniqueUser(registerDTO.Name!);
                if (!ifUnique)
                {
                    return BadRequest("Username already exist");
                }
                if (ModelState.IsValid)
                {
                    var register = await _userRepository.Register(registerDTO);
                    return Ok(register);
                }
                return BadRequest("wrong"); 
            }
            return BadRequest("Error in start code");   
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(string name)
        {
            if (ModelState.IsValid)
            {
                var delete = await _userRepository.DeleteUser(name);
                return Ok(delete);
            }
            return BadRequest("Wrong");
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (ModelState.IsValid)
            {
                await _signInManager.SignOutAsync();
                return Ok("Logout");
            }
            return BadRequest("Not logout");
        }
    }
}
