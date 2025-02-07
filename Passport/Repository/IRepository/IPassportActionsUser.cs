using Passport.Models;
using Passport.Models.DTO;
using System.Security.Claims;

namespace Passport.Repository.IRepository
{
    public interface IPassportActionsUser
    {
        Task<OperationResult> CreateOrder(SendOwnDataDTO sendOwnDataDTO);
        //Task<OperationResult> GetStatus(int id);
        Task<OperationResult> GetPassportDetails(int id/*, ClaimsPrincipal userClaims*/);
        Task<OperationResult> DeleteOrder(int id);
        Task<OperationResult> GetPass(int id);
    }
}
