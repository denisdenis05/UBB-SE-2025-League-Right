using Duo.Converters;
using System;
using Xunit;

using XunitAssert = Xunit.Assert;

namespace DuoTesting.Converters
{
    public class QuizCommandParameterConverterTests
    {
        [Fact]
        public void Convert_ValidInputs_ReturnsTuple()
        {
            var converter = new QuizCommandParameterConverter();
            int quizId = 42;
            bool isExam = true;

            var result = converter.Convert(quizId, typeof(object), isExam, "");

            XunitAssert.NotNull(result);
            XunitAssert.IsType<(int, bool)>(result);

            var tuple = ((int, bool))result;
            XunitAssert.Equal(42, tuple.Item1);
            XunitAssert.True(tuple.Item2);
        }

        [Fact]
        public void Convert_InvalidValueType_ReturnsNull()
        {
            var converter = new QuizCommandParameterConverter();

            var result = converter.Convert("not-an-int", typeof(object), true, "");

            XunitAssert.Null(result);
        }

        [Fact]
        public void Convert_InvalidParameterType_ReturnsNull()
        {
            var converter = new QuizCommandParameterConverter();

            var result = converter.Convert(42, typeof(object), "not-a-bool", "");

            XunitAssert.Null(result);
        }

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new QuizCommandParameterConverter();

            XunitAssert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(null, null, null, null));
        }
    }
}
