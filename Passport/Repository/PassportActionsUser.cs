using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Passport.Data;
using Passport.Models;
using Passport.Models.DTO;
using Passport.Repository.IRepository;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Passport.Repository
{
    public class PassportActionsUser : IPassportActionsUser
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PassportActionsUser(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize]
        public async Task<OperationResult> CreateOrder(SendOwnDataDTO sendOwnDataDTO)   
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User not authenticated"
                };
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.OrderId != null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User already has an order",
                    Errors = new List<string>()
                };
            }
            SendOwnData send = new()
            {
                FirstName = sendOwnDataDTO.FirstName,
                LastName = sendOwnDataDTO.LastName,
                DateOfBirth = sendOwnDataDTO.DateOfBirth,
                Nationality = sendOwnDataDTO.Nationality,
                Gender = sendOwnDataDTO.Gender,
                CityOfResidence = sendOwnDataDTO.CityOfResidence,
                PhotoUrl = sendOwnDataDTO.PhotoUrl,
                SubmittedDate = DateTime.UtcNow,
                UserId = int.Parse(userId)
            };
            _db.SendData.Add(send);
            await _db.SaveChangesAsync();

            user!.OrderId = send.Id;
            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();
            
            OperationResult result = new() 
            { 
                Succeeded = true,
                Message = "Order for creating passport is sended!",
                Data = send
            };
            return result;
            
        }

        public async Task<OperationResult> GetPassportDetails(int id)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User is not authenticated"
                };
            }

            var userIdPars = int.Parse(userId);            
            var userOrders = await _db.SendData.Include(o => o.User).Where(o => o.UserId == userIdPars).ToListAsync();
            if (userOrders != null)
            {
                OperationResult result = new()
                {
                    Succeeded = true,
                    Message = "Details",
                    Data = userOrders,
                    Errors = new List<string>()
                };
                return result;
            }
            else
            {
                OperationResult badRes = new()
                {
                    Succeeded = false,
                    Message = "Can't find ;("
                };
                return badRes;
            }
        }

        //public async Task<OperationResult> GetStatus(int id)
        //{
        //    var findStatus = await _db.SendData.FindAsync(id);
        //    if(findStatus != null)
        //    {
        //        OperationResult result = new()
        //        {
        //            Succeeded = true,
        //            Message = "Status",
        //            Data = findStatus?.Status,
        //            Errors = new List<string>()
        //        };
        //        return result;
        //    }
        //    else
        //    {
        //        OperationResult badRes = new()
        //        {
        //            Succeeded = false,
        //            Message = "Incorect id"
        //        };
        //        return badRes;
        //    }   
        //}

        public async Task<OperationResult> DeleteOrder(int id)
        {
            //added ---------------------
            var userId = _httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User is not authenticated"
                };
            }
            var userIdPars = int.Parse(userId);
            var userOrders = await _db.SendData.Include(o => o.User).Where(o => o.UserId == userIdPars).FirstOrDefaultAsync(x => x.Id == id);
            //added ---------------------

            //var data = await _db.SendData.FirstOrDefaultAsync(x => x.Id == id);
            if(userOrders != null)
            {
                _db.SendData.Remove(userOrders!);
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userIdPars);
                if (user != null)
                {
                    user.OrderId = null;    
                }
                await _db.SaveChangesAsync();
                OperationResult result = new()
                {
                    Succeeded = true,
                    Message = "Order deleted! ",
                    Data = userOrders,
                    Errors = new List<string>()
                };
                return result;
            }
            else
            {
                OperationResult badRes = new()
                {
                    Succeeded = false,
                    Message = "Order isn't deleted"
                };
                return badRes;
            }

        }

        public async Task<OperationResult> GetPass(int id)
        {
            var getData = await _db.SendData.FindAsync(id);
            if (getData == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Application not found",
                    Errors = new List<string>()
                };
            }
            var getPass = await _db.Passports.Include(p => p.sendOwnData).FirstOrDefaultAsync(p => p.SendOwnDataId == getData.Id);
            if (getPass != null)
            {
                OperationResult result = new()
                {
                    Succeeded = true,
                    Message = "Passport",
                    Data = getPass,
                    Errors = new List<string>()
                };
                return result;
            }
            else
            {
                OperationResult badRes = new()
                {
                    Succeeded = false,
                    Message = "Passport is not created",
                    Errors = new List<string>() 
                };
                return badRes;
            }
        }
    }
}
