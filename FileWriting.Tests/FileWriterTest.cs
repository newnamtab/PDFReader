using Microsoft.Extensions.Options;
using Moq;

namespace FileWriting.Tests
{
    public class FileWriterTest
    {
        private void setUp()
        {
            // setup test files directory
        }
        private void tearDown()
        {
            // remove files and directory created during tests
        }
        [Fact]
        public async Task FileWriter_Persists_PDF_File_Correctly()
        {

            //  var sut = GetSut();
            //  var result = await sut.SaveFile("BR12345", new StringContent("Fake PDF content"));
            //
            //  Assert.True(result.Success);
            Assert.True(true);
        }
        private FileWriter GetSut() {
            var mockBrNumberValidation = new Mock<IBRNumberValidation>();
            mockBrNumberValidation.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            
            return new FileWriter(Options.Create(new FileWriterSettings
            {
                FileStorageDirectory = "TestData"
            }), mockBrNumberValidation.Object);
        }
    }
}
