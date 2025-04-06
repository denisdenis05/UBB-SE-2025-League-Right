using Xunit;
using Duo.Converters;
using Microsoft.UI.Xaml;

using XunitAssert = Xunit.Assert; 

namespace DuoTesting.Converters
{
    public class BooleanToVisibilityConverterTests
    {
        private readonly BooleanToVisibilityConverter _converter;

        public BooleanToVisibilityConverterTests()
        {
            _converter = new BooleanToVisibilityConverter();
        }

        [Fact]
        public void Convert_ShouldReturnVisible_WhenValueIsTrue()
        {
            var result = _converter.Convert(true, null, null, null);
            XunitAssert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void Convert_ShouldReturnCollapsed_WhenValueIsFalse()
        {
            var result = _converter.Convert(false, null, null, null);
            XunitAssert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void Convert_ShouldReturnCollapsed_WhenValueIsNotBoolean()
        {
            var result = _converter.Convert("not_bool", null, null, null);
            XunitAssert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void ConvertBack_ShouldThrowNotSupportedException()
        {
            XunitAssert.Throws<NotSupportedException>(() => _converter.ConvertBack(null, null, null, null));
        }
    }
}
