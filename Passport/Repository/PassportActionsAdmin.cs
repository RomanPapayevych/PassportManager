using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Passport.Data;
using Passport.Models;
using Passport.Repository.IRepository;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;
using System.Text;

namespace Passport.Repository
{
    [Authorize(Roles = "Admin")]
    public class PassportActionsAdmin : IPassportActionsAdmin
    {
        private readonly ApplicationDbContext _db;
        public PassportActionsAdmin(ApplicationDbContext db) 
        {
            _db = db;
        }

        public async Task<OperationResult> CreatePassport(int id)
        {       
            var application = await _db.SendData.FindAsync(id);
            
            if (application == null || application.Status != "Pending")
            {
                OperationResult result1 = new()
                {
                    Succeeded = false,
                    Message = "Application not found or already processed..",
                    Errors = new List<string>()
                };
                return result1;
            }

            var existingPassport = await _db.Passports.FirstOrDefaultAsync(p => p.SendOwnDataId == application.Id);
            if (existingPassport != null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User already has a passport.",
                    Errors = new List<string>()
                };
            }

            var passport = new PassportOrigin
            {
                PassportNumber = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8),
                IdentificationCode = Guid.NewGuid().ToString(),
                IssueDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(10),
                sendOwnData =  application,
                SendOwnDataId = application.Id
            };

            application.Status = "Approved";
            _db.Passports.Add(passport);
            await _db.SaveChangesAsync();

            var createdPass = await _db.Passports.Include(p => p.sendOwnData).FirstOrDefaultAsync(p => p.Id == passport.Id);

            OperationResult result = new()
            {
                Succeeded = true,
                Message = "Passport created!",
                Data = createdPass,
                Errors = new List<string>()
            };
            return result;
        }

        public async Task<OperationResult> DeletePass(int id)
        {
            var pass = _db.Passports.FirstOrDefault(p => p.Id == id);
            if (pass == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Passport not found."
                };
            }
            var send = await _db.SendData.Include(p => p.Passports).FirstOrDefaultAsync(p => p.Id == pass.SendOwnDataId);
            if (send == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Related application not found."
                };
            }
            var user = await _db.Users.FirstOrDefaultAsync(u => u.OrderId == send.Id);
            if (user != null)
            {
                user.OrderId = null;
            }
            else
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "User with matching OrderId not found."
                };
            }
            _db.Passports.Remove(pass);
            _db.SendData.Remove(send);
            await _db.SaveChangesAsync();
            return new OperationResult
            {
                Succeeded = true,
                Message = "Passport deleted!",
                Errors = new List<string>()
            };
        }

        public async Task<OperationResult> RejectPass(int id)
        {
            var order = await _db.SendData.FirstOrDefaultAsync(p => p.Id == id);
            if (order != null)
            {
                order.Status = "Rejected";
                await _db.SaveChangesAsync();
                return new OperationResult
                {
                    Succeeded = true,
                    Message = "Order has been rejected"
                };
            }
            return new OperationResult
            {
                Succeeded = false,
                Message = "Invalid Id :/",
                Errors = new List<string>()
            };
        }

        public async Task<IEnumerable<SendOwnData>> GetOrders()
        {
            var getOrders = await _db.SendData.Where(n => n.Status=="Pending").Select(u => new SendOwnData
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                Nationality = u.Nationality,
                Gender = u.Gender,
                CityOfResidence = u.CityOfResidence,
                PhotoUrl = u.PhotoUrl,
                Status = u.Status,
                SubmittedDate = u.SubmittedDate
            }).ToListAsync();
            return getOrders;
        }

        public async Task<OperationResult> GetPassport()
        {
            var getPass = await _db.Passports.Include(p => p.sendOwnData).ToListAsync();
            if(getPass != null)
            {
                return new OperationResult
                {
                    Succeeded = true,
                    Message = "Pasports",
                    Data = getPass,
                    Errors = new List<string>()
                };
            }
            return new OperationResult
            {
                Succeeded = false,
                Message = "Passport list is empty(",
                Errors = new List<string>()
            };
        }

        public async Task<OperationResult> GeneratePdfReport(int id)
        {
            var passport = await _db.Passports.Include(p => p.sendOwnData).FirstOrDefaultAsync(p => p.Id == id);
            if (passport == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Passport not found.",
                    Errors = new List<string>()
                };
            }

            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 12);
            var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
            var subtitleFont = new XFont("Arial", 16, XFontStyle.Bold);
            var textFont = new XFont("Arial", 12, XFontStyle.Regular);
            var linePen = new XPen(XColors.LightGray, 1);

            gfx.DrawString("Passport Report", titleFont, XBrushes.DarkBlue, new XPoint(40, 40));
            gfx.DrawLine(linePen, 40, 60, page.Width - 40, 60);

            gfx.DrawString("Passport Details", subtitleFont, XBrushes.Black, new XPoint(40, 80));
            gfx.DrawString($"Passport Number: {passport.PassportNumber}", textFont, XBrushes.Black, new XPoint(40, 130));
            gfx.DrawString($"Identification Code: {passport.IdentificationCode}", textFont, XBrushes.Black, new XPoint(40, 150));
            gfx.DrawString($"Issue Date: {passport.IssueDate.ToShortDateString()}", textFont, XBrushes.Black, new XPoint(40, 170));
            gfx.DrawString($"Expiry Date: {passport.ExpiryDate.ToShortDateString()}", textFont, XBrushes.Black, new XPoint(40, 190));

            gfx.DrawString("Passport Holder Information", subtitleFont, XBrushes.Black, new XPoint(40, 230));
            gfx.DrawString($"Full Name: {passport.sendOwnData!.FirstName} {passport.sendOwnData.LastName}", textFont, XBrushes.Black, new XPoint(40, 260));
            gfx.DrawString($"Date of Birth: {passport.sendOwnData.DateOfBirth.ToShortDateString()}", textFont, XBrushes.Black, new XPoint(40, 280));
            gfx.DrawString($"Nationality: {passport.sendOwnData.Nationality}", textFont, XBrushes.Black, new XPoint(40, 300));
            gfx.DrawString($"Gender: {passport.sendOwnData.Gender}", textFont, XBrushes.Black, new XPoint(40, 320));
            gfx.DrawString($"City of Residence: {passport.sendOwnData.CityOfResidence}", textFont, XBrushes.Black, new XPoint(40, 340));

            gfx.DrawLine(linePen, 40, 360, page.Width - 40, 360);
            gfx.DrawString($"Report generated on: {DateTime.Now.ToShortDateString()}", textFont, XBrushes.Gray, new XPoint(40, 390));

            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                var pdfBytes = stream.ToArray();

                return new OperationResult
                {
                    Succeeded = true,
                    Message = "PDF Report generated.",
                    Data = pdfBytes,
                    Errors = new List<string>()
                };
            }
        }
        public async Task<OperationResult> GenerateCsvReport(int id)
        {
            var passport = await _db.Passports.Include(p => p.sendOwnData).FirstOrDefaultAsync(p => p.Id == id);
            if (passport == null)
            {
                return new OperationResult
                {
                    Succeeded = false,
                    Message = "Passport not found.",
                    Errors = new List<string>()
                };
            }
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Passport Number,Identification Code,Issue Date,Expiry Date,Full Name,Date of Birth,Nationality,Gender,City of Residence");

           
                csvBuilder.AppendLine(
                    $"{passport.PassportNumber}," +
                    $"{passport.IdentificationCode}," +
                    $"{passport.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                    $"{passport.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                    $"{passport.sendOwnData!.FirstName} {passport.sendOwnData.LastName}," +
                    $"{passport.sendOwnData.DateOfBirth.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                    $"{passport.sendOwnData.Nationality}," +
                    $"{passport.sendOwnData.Gender}," +
                    $"{passport.sendOwnData.CityOfResidence}"
                );

            var csvBytes = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return new OperationResult
            {
                Succeeded = true,
                Message = "PDF Report generated.",
                Data = csvBytes,
                Errors = new List<string>()
            };
        }
        public async Task<OperationResult> GetTotalPassportCount()
        {
            var count =  await _db.Passports.CountAsync();
            return new OperationResult
            {
                Succeeded = true,
                Data = count,
                Message = "Count of created passports",
                Errors = new List<string>()
            };
        }
    }
}
