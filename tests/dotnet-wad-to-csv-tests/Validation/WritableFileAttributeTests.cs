using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using DotNet.AzureDiagnostics.Core.Validation;
using FluentAssertions;
using Xunit;

namespace DotNet.WadToCsv.Tests.Validation
{
    public class WritableFileAttributeTests
    {
        private static WritableFileAttribute _target;
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

            actual?.ErrorMessage.Should().EndWith("The value is not a string.");
        }

        [Fact]
        public void GivenNonWritableFilePath_WhenGetValidationResult_ThenErrorMessage()
        {
            // Arrange

            const string nonWritableFilePath = "   ";

            SetTarget(nonWritableFilePath);

            // Act

            var actual = _target.GetValidationResult(nonWritableFilePath, _validationContext);

            // Assert

            actual?.ErrorMessage.Should().Contain("Exception");
            actual?.ErrorMessage.Should().Contain("Message");
        }

        [Fact]
        public void GivenWritableFilePath_WhenGetValidationResult_ThenNoErrorMessage()
        {
            // Arrange

            var writableFilePath = Path.GetTempFileName();

            SetTarget(writableFilePath);

            // Act

            var actual = _target.GetValidationResult(writableFilePath, _validationContext);

            // Assert

            actual.Should().BeNull();
        }

        private static void SetTarget(object value)
        {
            _target = new WritableFileAttribute();
            _validationContext = new ValidationContext(value);
        }
    }
}
