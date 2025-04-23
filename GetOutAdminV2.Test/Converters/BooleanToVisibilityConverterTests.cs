using System;
using System.Globalization;
using System.Windows;
using GetOutAdminV2.Converters;
using Xunit;

namespace GetOutAdminV2.Tests.Converters
{
    public class BooleanToVisibilityConverterTests
    {
        [Fact]
        public void Convert_WhenTrue_ReturnsVisible()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();

            // Act
            var result = converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void Convert_WhenFalse_ReturnsCollapsed()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();

            // Act
            var result = converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void Convert_WithInverseParameter_ReturnsOppositeVisibility()
        {
            // Arrange
            var converter = new BooleanToVisibilityConverter();

            // Act
            var resultTrue = converter.Convert(true, typeof(Visibility), "inverse", CultureInfo.InvariantCulture);
            var resultFalse = converter.Convert(false, typeof(Visibility), "inverse", CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(Visibility.Collapsed, resultTrue);
            Assert.Equal(Visibility.Visible, resultFalse);
        }
    }
}