using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Passport.Models;
using Passport.Repository.IRepository;

namespace Passport.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IPassportActionsAdmin _passportAdmin;
        public AdminController(IPassportActionsAdmin passportAdmin)
        {
            _passportAdmin = passportAdmin;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (ModelState.IsValid)
            {
                var getOrders = await _passportAdmin.GetOrders();
                return Ok(getOrders);
            }
            return BadRequest("Wrong");
        }
        [HttpPost("{id}/createPass")]
        public async Task<IActionResult> CreatePass(int id)
        {
            if (ModelState.IsValid)
            {
                var create = await _passportAdmin.CreatePassport(id);
                return Ok(create);
            }
            return BadRequest("huinia");
        }

        [HttpGet("getPass")]
        public async Task<IActionResult> GetPassport()
        {
            if (ModelState.IsValid)
            {
                var getPass = await _passportAdmin.GetPassport();
                return Ok(getPass);
            }
            return BadRequest("Passport list is empty");
        }

        [HttpDelete("{id}/deletePass")]
        public async Task<IActionResult> DeletePass(int id)
        {
            if (ModelState.IsValid)
            {
                var delete = await _passportAdmin.DeletePass(id);
                return Ok(delete);
            }
            return BadRequest("Invalid id");
        }
        [HttpDelete("{id}/rejectPass")]
        public async Task<IActionResult> RejectPass(int id)
        {
            if (ModelState.IsValid)
            {
                var delete = await _passportAdmin.RejectPass(id);
                return Ok(delete);
            }
            return BadRequest("Invalid id");
        }

        [HttpGet("{id}/GenerateReport")]
        public async Task<IActionResult> GenerateReport(int id)
        {
            var report = await _passportAdmin.GeneratePdfReport(id);
            if (!report.Succeeded)
            {
                return BadRequest(report.Message);
            }
            var pdfBytes = (byte[])report.Data!;
            return File(pdfBytes, "application/pdf", "passport_report.pdf");
        }
        [HttpGet("{id}/GenerateCsvReport")]
        public async Task<IActionResult> GenerateCsvReport(int id)
        {
            var report = await _passportAdmin.GenerateCsvReport(id);
            if (!report.Succeeded)
            {
                return BadRequest(report.Message);
            }
            var csvBytes = (byte[])report.Data!;
            return File(csvBytes, "text/csv", "passport_report.csv");
        }
        [HttpGet("GetTotalPassportCount")]
        public async Task<IActionResult> GetTotalPassportCount()
        {
            var count = await _passportAdmin.GetTotalPassportCount();
            if (!count.Succeeded)
            {
                return BadRequest(count.Message);
            }
            return Ok(count);
        }

    }
}
