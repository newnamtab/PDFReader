namespace URLProvider.Tests
{
    public class ExcelFileReadTests
    {
        [Fact]
        public void Valid_ExcelFile_Returns_2_Urls()
        {
            var sut = GetSut();

            var urls = sut.GetURLs().ToArray();

            Assert.NotNull(urls);
            Assert.Equal(2, urls.Length);

            Assert.Equal("BR1000", urls[0].BRNumber);
            Assert.Equal("http://PrimaryColumnURL.One", urls[0].PrimaryUrl);
            Assert.Equal("http://SecondaryColumnURL.One", urls[0].SecondaryUrl);

            Assert.Equal("BR2000", urls[1].BRNumber);
            Assert.Equal("http://PrimaryColumnURL.Two", urls[1].PrimaryUrl);
            Assert.Equal("http://SecondaryColumnURL.Two", urls[1].SecondaryUrl);
        }
        private URLProvider GetSut()
        {
            return new URLProvider("TestData/ValidExcelFile.xlsx","BRNumberColumn", "PrimaryColumn", "SecondaryColumn");
        }
    }
}
