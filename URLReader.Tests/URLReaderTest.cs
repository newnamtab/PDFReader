using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;

namespace URLReader.Tests
{
    public class URLReaderTest
    {
        [Fact]
        public async Task UrlReader_Gets_Valid_Content()
        {
            var sut = GetSut();

            var result = await sut.ReadURL(new Uri("http://fakeUriForTest.com"));

            Assert.NotNull(result);
            Assert.True(result.Success);

            var contentString = await result.PDFContent.ReadAsStringAsync();
            Assert.Equal("Test content", contentString);

        }
        private URLReader GetSut()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("Test content",
                                                Encoding.UTF8,
                                                "application/pdf"),
               })
               .Verifiable();
            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object);
            
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return new URLReader(httpClientFactoryMock.Object);
        }

        //[Fact]
        //public async Task UrlReader_Gets_Valid_Content_For_Realz()
        //{
        //    var sut = GetSutFoRealz();
        //
        //    var result = await sut.ReadURL(new Uri("http://arpeissig.at/wp-content/uploads/2016/02/D7_NHB_ARP_Final_2.pdf"));
        //
        //    Assert.NotNull(result);
        //
        //    var contentString = await result.PDFContent.ReadAsStringAsync();
        //    Assert.NotNull (contentString);
        //}
        //private URLReader GetSutFoRealz()
        //{
        //    var services = new ServiceCollection();
        //    services.AddHttpClient("pdfClient", x => { x.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/pdf")); });
        //    var actualHttpClientFactory = services
        //                                  .BuildServiceProvider()
        //                                  .GetRequiredService<IHttpClientFactory>();
        //
        //    return new URLReader(actualHttpClientFactory);
        //}
    }
}
