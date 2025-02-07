using Microsoft.AspNetCore.Mvc;
using Passport.Models;

namespace Passport.Repository.IRepository
{
    public interface IPassportActionsAdmin
    {
        Task<IEnumerable<SendOwnData>> GetOrders();
        Task<OperationResult> CreatePassport(int id);
        Task<OperationResult> DeletePass(int id);
        Task<OperationResult> GetPassport();
        Task<OperationResult> RejectPass(int id);

        Task<OperationResult> GeneratePdfReport(int id);
        Task<OperationResult> GenerateCsvReport(int id);
        Task<OperationResult> GetTotalPassportCount();
    }
}
