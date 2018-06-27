using System;
using System.ComponentModel.DataAnnotations;
using DotNet.AzureDiagnostics.Core.Validation;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Validation
{
    public class Iso8601DateTimeAttributeTests
    {
        private static Iso8601DateTimeAttribute _target;
        private static ValidationContext _validationContext;

        [Fact]
        public void GivenNonString_WhenGetValidationResult_ThenErrrorMessage()
        {
            // Arrange

            var iAmOfTheWrongType = DateTime.MaxValue;

            SetTarget(iAmOfTheWrongType);

            // Act

            var actual = _target.GetValidationResult(iAmOfTheWrongType, _validationContext);

            // Assert

            actual?.ErrorMessage.Should().EndWith("is not a valid ISO 8601 date time. Refer to https://en.wikipedia.org/wiki/ISO_8601");
        }

        [Fact]
        public void GivenInvalidDateTime_WhenGetValidationResult_ThenErrorMessage()
        {
            // Arrange

            const string invalidDateTime = "Hello";

            SetTarget(invalidDateTime);

            // Act

            var actual = _target.GetValidationResult(invalidDateTime, _validationContext);

            // Assert

            actual?.ErrorMessage.Should().EndWith("is not a valid ISO 8601 date time. Refer to https://en.wikipedia.org/wiki/ISO_8601");
        }

        [Fact]
        public void GivenValidDateTime_WhenGetValidationResult_ThenNoErrorMessage()
        {
            // Arrange

            const string validDateTime = "2018-06-24T21:03:05";

            SetTarget(validDateTime);

            // Act

            var actual = _target.GetValidationResult(validDateTime, _validationContext);

            // Assert

            actual.Should().BeNull();
        }

        private static void SetTarget(object value)
        {
            _target = new Iso8601DateTimeAttribute();
            _validationContext = new ValidationContext(value);
        }
    }
}
