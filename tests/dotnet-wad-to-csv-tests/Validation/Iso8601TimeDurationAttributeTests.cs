using System;
using System.ComponentModel.DataAnnotations;
using DotNet.WadToCsv.Validation;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Validation
{
    public class Iso8601TimeDurationAttributeTests
    {
        private static Iso8601TimeDurationAttribute _target;
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

            actual?.ErrorMessage.Should().EndWith("is not a valid ISO 8601 time duration. Refer to the time component of https://en.wikipedia.org/wiki/ISO_8601#Durations");
        }

        [Fact]
        public void GivenInvalidDuration_WhenGetValidationResult_ThenErrorMessage()
        {
            // Arrange

            const string invalidDuration = "Hello";

            SetTarget(invalidDuration);

            // Act

            var actual = _target.GetValidationResult(invalidDuration, _validationContext);

            // Assert

            actual?.ErrorMessage.Should().EndWith("is not a valid ISO 8601 time duration. Refer to the time component of https://en.wikipedia.org/wiki/ISO_8601#Durations");
        }

        [Fact]
        public void GivenValidDuration_WhenGetValidationResult_ThenNoErrorMessage()
        {
            // Arrange

            const string validDuration = "5H7M9S";

            SetTarget(validDuration);

            // Act

            var actual = _target.GetValidationResult(validDuration, _validationContext);

            // Assert

            actual.Should().BeNull();
        }

        private static void SetTarget(object value)
        {
            _target = new Iso8601TimeDurationAttribute();
            _validationContext = new ValidationContext(value);
        }
    }
}
