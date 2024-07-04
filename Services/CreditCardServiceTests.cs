using CreditCardValidationAPI.Services;
using System;
using Xunit;

namespace CreditCardValidationTests
{
    public class CreditCardServiceTests
    {
        private readonly CreditCardService _service;

        public CreditCardServiceTests()
        {
            _service = new CreditCardService();
        }

        [Theory]
        [InlineData("4532015112830366", true)] // Valid Visa card.
        [InlineData("6011514433546201", true)] // Valid Discover card
        [InlineData("1234567890123456", false)] // Invalid card
        public void ValidateLuhn_ShouldValidateCorrectly(string cardNumber, bool expected)
        {
            var result = _service.ValidateLuhn(cardNumber);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidateLuhn_ShouldThrowExceptionForNullOrEmptyInput()
        {
            Assert.Throws<ArgumentException>(() => _service.ValidateLuhn(null));
            Assert.Throws<ArgumentException>(() => _service.ValidateLuhn(""));
        }

        [Fact]
        public void ValidateLuhn_ShouldThrowExceptionForNonDigitInput()
        {
            Assert.Throws<ArgumentException>(() => _service.ValidateLuhn("abc123"));
        }
    }
}
