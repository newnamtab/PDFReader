using System.Net.Mime;

namespace URLReader
{
    public interface IURLReader
    {
        Task<PDFContentResult> ReadURL(Uri uri);
    }
    public class URLReader : IURLReader
    {
        private readonly HttpClient _httpClient;

        public URLReader(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("pdfClient");
        }
        public async Task<PDFContentResult> ReadURL(Uri uri)
        {
            return await DownloadPDF(uri);
        }
        private async Task<PDFContentResult> DownloadPDF(Uri uri)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };
            
            using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                var response = await _httpClient.SendAsync(request, timeoutCancellationTokenSource.Token);
                if (response.IsSuccessStatusCode == false)
                {
                    return new PDFContentResult(false, new StringContent(string.Empty) );
                }

                if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == MediaTypeNames.Application.Pdf)
                {
                    // actually a System.Net.Http.StreamContent instance but you do not need to cast as the actual type does not matter in this case;
                    return new PDFContentResult(true, response.Content); 
                }
            }
            catch (TaskCanceledException tcex)
            {
                // LOG IT SOMEWHERE (To be determined)
                Console.WriteLine("CancellationToken expired: ", tcex.Message);
            }
            catch (Exception e)
            {
                // LOG IT SOMEWHERE (To be determined)
                Console.WriteLine("Error Reading pdf: ", e.Message);
            }
            return new PDFContentResult(false, new StringContent(string.Empty));
        }
    }

    public record PDFContentResult(bool Success, HttpContent PDFContent);
    
}
