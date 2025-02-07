using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Passport.Controllers;
using Passport.Models;
using Passport.Models.DTO;
using Passport.Repository.IRepository;
using Xunit;
namespace PassportTests
{
    public class UserControllerTests
    {
        private readonly UserController? _userController;
        private readonly Mock<IPassportActionsUser>? _passportActionsUser;
        public UserControllerTests()
        {
            _passportActionsUser = new Mock<IPassportActionsUser>();
            _userController = new UserController(_passportActionsUser.Object);
        }
        [Fact]
        public async Task CreateOrder_Succeeded()
        {
            // Arrange
            var sendOwnDataDTO = new SendOwnDataDTO
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 1, 1),
                Nationality = "USA",
                Gender = "Male",
                CityOfResidence = "New York",
                PhotoUrl = "http://example.com/photo.jpg"
            };
            var expectedResult = new OperationResult
            {
                Succeeded = true,
                Message = "Order for creating passport is sended!"
            };

            _passportActionsUser?.Setup(repo => repo.CreateOrder(sendOwnDataDTO)).ReturnsAsync(expectedResult);

            // Act
            var result = await _userController!.CreateOrder(sendOwnDataDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<OperationResult>(okResult.Value);
            Assert.True(expectedResult.Succeeded);
            Assert.Equal(expectedResult.Succeeded, resultValue.Succeeded);

        }

        [Fact]
        public async Task CreateOrder_BadRequest()
        {
            // Arrange
            _userController!.ModelState.AddModelError("FirstName", "First name is required");

            var sendOwnDataDTO = new SendOwnDataDTO();

            // Act
            var result = await _userController!.CreateOrder(sendOwnDataDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Go wrong :(", badRequestResult.Value);
        }
        [Fact]
        public async Task Delete_Valid()
        {
            //Arrange
            int id = 1;
            var expected = new OperationResult
            {
                Succeeded = true,
                Message = "Deleted"
            };

            _passportActionsUser?.Setup(repo => repo.DeleteOrder(id)).ReturnsAsync(expected);
            //Act
            var result = await _userController!.Delete(id);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<OperationResult>(okResult.Value);
            Assert.Equal(expected.Succeeded, resultValue.Succeeded);
            Assert.Equal(expected.Message, resultValue.Message);
        }
        [Fact]
        public async Task Delete_Invalid()
        {
            //Arrange
            int id = -1;
            var expected = new OperationResult
            {
                Succeeded = false,
                Message = "Invalid delete"
            };

            _passportActionsUser?.Setup(repo => repo.DeleteOrder(id)).ReturnsAsync(expected);
            //Act
            var result = await _userController!.Delete(id);

            //Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            var resultValue = Assert.IsType<OperationResult>(okResult.Value);
            Assert.False(resultValue.Succeeded);
            Assert.Equal("Invalid delete", resultValue.Message);
        }
        [Fact]
        public async Task Details_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            int id = 1;
            _userController?.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _userController!.Details(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something go wrong", badRequestResult.Value);
        }
        [Fact]
        public async Task GetPass_ValidId_ReturnsOkResult()
        {
            // Arrange
            int id = 1;
            var expectedPassport = new PassportOrigin();
            var expectedPass = new OperationResult 
            {
                Succeeded = true,
                Message = "Success",
                Data = expectedPassport

            };
            _passportActionsUser?.Setup(x => x.GetPass(id)).ReturnsAsync(expectedPass);

            // Act
            var result = await _userController!.GetPass(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<OperationResult>(okResult.Value);
            Assert.True(returnValue.Succeeded);
            Assert.Equal("Success", returnValue.Message);

            Assert.Equal(expectedPass.ToString(), returnValue.ToString()); 
        }
        [Fact]
        public async Task GetPass_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            int id = -1; 
            var expectedResult = new OperationResult
            {
                Succeeded = false,
                Message = "Invalid ID",
                Data = null
            };
            _passportActionsUser?.Setup(x => x.GetPass(id)).ReturnsAsync(expectedResult);

            // Act
            var result = await _userController!.GetPass(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Something go wrong", badRequestResult.Value);
        }
    }
}
