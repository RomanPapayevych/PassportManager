using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Passport.Data;
using Passport.Models;
using Passport.Models.DTO;
using Passport.Repository.IRepository;
using System.Security.Claims;

namespace Passport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IPassportActionsUser _passportActionsUser;
        public UserController(IPassportActionsUser passportActionsUser)
        {
            _passportActionsUser = passportActionsUser;
        }
        [Authorize]
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] SendOwnDataDTO sendOwnData)
        {
            if (ModelState.IsValid)
            {   
                var createOrder = await _passportActionsUser.CreateOrder(sendOwnData);
                return Ok(createOrder);
            }
            return BadRequest("Go wrong :(");
        }

        [HttpGet("{id}/details")]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (ModelState.IsValid)
            {
                var details = await _passportActionsUser.GetPassportDetails(id);
                return Ok(details);
            }
            return BadRequest("Something go wrong");
        }

        [HttpDelete("{id}/delete")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var delete = await _passportActionsUser.DeleteOrder(id);
                if (!delete.Succeeded)
                {
                    return new NotFoundObjectResult(delete);
                }
                return Ok(delete);
            }
            return BadRequest("Something go wrong");
        }
        [HttpGet("{id}/getPass")]
        [Authorize]
        public async Task<IActionResult> GetPass(int id)
        {
            if (ModelState.IsValid)
            {
                var get = await _passportActionsUser.GetPass(id);
                if (!get.Succeeded)
                {
                    return BadRequest("Something go wrong");
                }
                return Ok(get);
            }
            return BadRequest("Something go wrong");
        }
    }
}
