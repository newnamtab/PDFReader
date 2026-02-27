using Moq;
using PdfUrlExaminer;
using URLReader;

namespace PDFUrlExaminer.Tests
{
    public class PdfUrlExaminerTest
    {
        [Fact]
        public async Task PdfUrlExaminer_Reads_Valid_PDF_Correctly()
        {
            var sut = GetSut();
            var urlSets = new List<(string brNumber, string primaryUrl, string secondaryUrl)>
            {
                ("BR123", "http://validPrimaryUrl.com", "http://validSecondaryUrl.com")
            };

            await sut.ExamineUrls(urlSets);

            Assert.NotEmpty(urlSets);
        }
        private PdfUrlExaminer.PdfUrlExaminer GetSut()
        {
            var urlReaderMock = new Mock<IURLReader>(MockBehavior.Strict);
            urlReaderMock.Setup(r => r.ReadURL(It.IsAny<Uri>())).ReturnsAsync(new PDFContentResult(true, new StringContent("Fake PDF content")));

            var fileWriterMock = new Mock<IFileWriter>(MockBehavior.Strict);
            fileWriterMock.Setup(f => f.SaveFile(It.IsAny<string>(), It.IsAny<HttpContent>())).Returns(Task.CompletedTask);

            return new PdfUrlExaminer.PdfUrlExaminer(urlReaderMock.Object, fileWriterMock.Object);
        }
    }
}
