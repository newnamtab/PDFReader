using FileWriting;
using Microsoft.Extensions.Logging;
using URLReader;

namespace PdfUrlExaminer
{
    public interface IPdfUrlExaminer
    {
        Task ExamineUrls(IEnumerable<(string brNumber, string primaryUrl, string secondaryUrl)> urlSets);
    }

    public class PdfUrlExaminer : IPdfUrlExaminer
    {
        private readonly IURLReader _pdfUrlReader;
        private readonly IFileWriter _fileWriter;
        private readonly ILogger<PdfUrlExaminer> _logger;

        public PdfUrlExaminer(IURLReader pdfUrlReader, IFileWriter fileWriter, ILogger<PdfUrlExaminer> logger)
        {
            _pdfUrlReader = pdfUrlReader;
            _fileWriter = fileWriter;
            _logger = logger;
        }
        public async Task ExamineUrls(IEnumerable<(string brNumber, string primaryUrl, string secondaryUrl)> urlSets)
        {
            foreach (var urlSet in urlSets)
            {
                await BuildExamineUrlTask(urlSet.brNumber, urlSet.primaryUrl, urlSet.secondaryUrl);
            }
            // var throttler = new SemaphoreSlim(10);
            // var tasks = urlSets.Select(async urlSet =>
            // {
            //     await throttler.WaitAsync();
            //     try { await BuildExamineUrlTask(urlSet.brNumber, urlSet.primaryUrl, urlSet.secondaryUrl); }
            //     finally { throttler.Release(); }
            // });
            // await Task.WhenAll(tasks);
        }
        private async Task BuildExamineUrlTask(string brNumber, string primaryUrl, string secondaryUrl)
        {
            var pdfReadResult = await ReadUrls(primaryUrl, secondaryUrl);
            if (pdfReadResult.Success)
            {
                var fileSaved = await SavePdf(brNumber, pdfReadResult.PdfContent);
                LogReport(brNumber, fileSaved);
                return;
            }
            LogReport(brNumber, false);
        }

        private async Task<PdfContentResult> ReadUrls(string primaryUrl, string secondaryUrl)
        {
            //prøv at hente PDF-indholdet fra primaryUrl eller secondaryUrl
            var primaryHttpContent = await TryReadUrl(primaryUrl);
            if (primaryHttpContent.Success)
            {
                return primaryHttpContent;
            }

            return await TryReadUrl(secondaryUrl);
        }
        private async Task<PdfContentResult> TryReadUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var validUri))
            {
                var pdfResult = await _pdfUrlReader.ReadURL(validUri);
                if (pdfResult.Success)
                {
                    return PdfContentResult.CreateSuccess(pdfResult.PDFContent);
                }
                return PdfContentResult.CreateFailure();
            }
            return PdfContentResult.CreateFailure();
        }
        private async Task<bool> SavePdf(string brNumber, HttpContent content)
        {
            await _fileWriter.SaveFile(brNumber, content);
            return true;
        }
        private void LogReport(string brNumber, bool success)
        {
            //Log en rapport om hvilke URL-er som blev forsøgt, og hvilke som fungerede
            _logger.LogInformation("Report: {BRNumber} => {Result}", brNumber, success ? "JA" : "NEJ");
        }
    }
    internal class PdfContentResult
    {
        public bool Success { get; private set; }
        public HttpContent PdfContent { get; private set; }

        private PdfContentResult(bool success, HttpContent pdfContent)
        {
            Success = success;
            PdfContent = pdfContent;
        }
        public static PdfContentResult CreateSuccess(HttpContent pdfContent)
        {
            return new PdfContentResult(true, pdfContent);
        }
        public static PdfContentResult CreateFailure()
        {
            return new PdfContentResult(false, new StringContent(string.Empty));
        }
    }
}
