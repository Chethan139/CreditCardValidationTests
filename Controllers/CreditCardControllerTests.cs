using CreditCardValidationAPI.Controllers;
using CreditCardValidationAPI.Models;
using CreditCardValidationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CreditCardValidationTests
{
    public class CreditCardControllerTests
    {
        private readonly Mock<CreditCardService> _mockService;
        private readonly CreditCardController _controller;

        public CreditCardControllerTests()
        {
            _mockService = new Mock<CreditCardService>();
            _controller = new CreditCardController(_mockService.Object);
        }

        [Fact]
        public void ValidateCreditCard_ShouldReturnBadRequest_WhenCardNumberIsNull()
        {
            // Arrange
            var creditCard = new CreditCard { CardNumber = null };

            // Act
            var result = _controller.ValidateCreditCard(creditCard);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Credit card number is required.", badRequestResult.Value);
        }

        [Fact]
        public void ValidateCreditCard_ShouldReturnBadRequest_WhenCardNumberIsEmpty()
        {
            // Arrange
            var creditCard = new CreditCard { CardNumber = "" };

            // Act
            var result = _controller.ValidateCreditCard(creditCard);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Credit card number is required.", badRequestResult.Value);
        }

        [Fact]
        public void ValidateCreditCard_ShouldReturnBadRequest_WhenCardNumberIsInvalid()
        {
            // Arrange
            var creditCard = new CreditCard { CardNumber = "abc123" };
            _mockService.Setup(s => s.ValidateLuhn(It.IsAny<string>())).Throws(new ArgumentException("Card number must contain only digits."));

            // Act
            var result = _controller.ValidateCreditCard(creditCard);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Card number must contain only digits.", badRequestResult.Value);
        }

        [Fact]
        public void ValidateCreditCard_ShouldReturnOk_WhenCardNumberIsValid()
        {
            // Arrange
            var creditCard = new CreditCard { CardNumber = "4532015112830366" };
            _mockService.Setup(s => s.ValidateLuhn(creditCard.CardNumber)).Returns(true);

            // Act
            var result = _controller.ValidateCreditCard(creditCard);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<CreditCardValidationResult>(okResult.Value);
            Assert.True(response.IsValid);
        }

        [Fact]
        public void ValidateCreditCard_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            var creditCard = new CreditCard { CardNumber = "4532015112830366" };
            _mockService.Setup(s => s.ValidateLuhn(It.IsAny<string>())).Throws(new Exception("Unexpected error"));

            // Act
            var result = _controller.ValidateCreditCard(creditCard);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
            Assert.Equal("An unexpected error occurred.", internalServerErrorResult.Value);
        }
    }
}
