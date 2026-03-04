using System;
using System.Collections.Generic;
using System.Text;

namespace FileWriting.Tests
{
    public class BRNumberValidationTests
    {
        [Theory]
        [InlineData("BR12345", true)]

        [InlineData("Br12345", false)]
        [InlineData("bR12345", false)]
        [InlineData("br12345", false)]

        [InlineData("BR1234X", false)]
        [InlineData("BRX2345", false)]
        [InlineData("BR1234" , false)]
        [InlineData("BR123456", false)]
        [InlineData("BRXXXXX", false)]
        [InlineData("1234567", false)]
        public void IsValid_Returns_Correct_Result(string brNumber, bool expected)
        {
            var sut = new BRNumberValidation(@"\bBR\d{5}\b");
            var result = sut.IsValid(brNumber);
            Assert.Equal(expected, result);
        }
    }
}
